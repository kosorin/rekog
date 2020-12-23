using Rekog.Core.Corpora;
using Rekog.Core.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Rekog.Core.Layouts.Analyzers
{
    public class HandAlternationLayoutAnalyzer : BigramLayoutAnalyzer<bool>
    {
        public HandAlternationLayoutAnalyzer() : base("Hand alternation")
        {
        }

        protected override bool TryGet(Key firstKey, Key secondKey, [MaybeNullWhen(false)] out bool value)
        {
            value = firstKey.Hand != secondKey.Hand;
            return true;
        }
    }
    public class NgramAnalyzer
    {
        public void Analyze(CorpusAnalysis corpusAnalysis, Layout layout)
        {
            var handAlternationOccurrences = new OccurrenceCollection<bool>();
            var sameHandRowJumpOccurrences = new OccurrenceCollection<(int firstRow, int secondRow)>();
            var sameHandRowJumpDistanceOccurrences = new OccurrenceCollection<int>();
            var handRollOccurrences = new OccurrenceCollection<(Hand hand, Roll roll)>();
            var rollOccurrences = new OccurrenceCollection<Roll>();
            var fingerMotionOccurrences = new OccurrenceCollection<(Finger finger, Motion motion)>();
            var handMotionOccurrences = new OccurrenceCollection<(Hand hand, Motion motion)>();
            var motionOccurrences = new OccurrenceCollection<Motion>();
            var sameFingerBigramOccurrences = new OccurrenceCollection<Finger>();
            var sameFingerBigramHandOccurrences = new OccurrenceCollection<Hand>();
            var neighborFingerBigramOccurrences = new OccurrenceCollection<(Finger firstFinger, Finger secondFinger)>();
            var neighborFingerBigramHandOccurrences = new OccurrenceCollection<Hand>();

            foreach (var bigram in corpusAnalysis.Bigrams)
            {
                if (!layout.TryGetKey(bigram.Value[0], out var firstKey) || !layout.TryGetKey(bigram.Value[1], out var secondKey))
                {
                    handAlternationOccurrences.AddNull(bigram.Count);
                    sameHandRowJumpOccurrences.AddNull(bigram.Count);
                    sameHandRowJumpDistanceOccurrences.AddNull(bigram.Count);
                    handRollOccurrences.AddNull(bigram.Count);
                    rollOccurrences.AddNull(bigram.Count);
                    fingerMotionOccurrences.AddNull(bigram.Count);
                    handMotionOccurrences.AddNull(bigram.Count);
                    motionOccurrences.AddNull(bigram.Count);
                    sameFingerBigramOccurrences.AddNull(bigram.Count);
                    sameFingerBigramHandOccurrences.AddNull(bigram.Count);
                    neighborFingerBigramOccurrences.AddNull(bigram.Count);
                    neighborFingerBigramHandOccurrences.AddNull(bigram.Count);
                    continue;
                }

                if (firstKey.Finger == secondKey.Finger && firstKey.Character != secondKey.Character)
                {
                    sameFingerBigramOccurrences.Add(firstKey.Finger, bigram.Count);
                }
                else
                {
                    sameFingerBigramOccurrences.AddNull(bigram.Count);
                }

                if (firstKey.Finger.IsNeighbor(secondKey.Finger))
                {
                    neighborFingerBigramOccurrences.Add((firstKey.Finger, secondKey.Finger), bigram.Count);
                }
                else
                {
                    neighborFingerBigramOccurrences.AddNull(bigram.Count);
                }

                handAlternationOccurrences.Add(firstKey.Hand != secondKey.Hand, bigram.Count);

                if (firstKey.Hand == secondKey.Hand && firstKey.Row != secondKey.Row)
                {
                    sameHandRowJumpOccurrences.Add((firstKey.Row, secondKey.Row), bigram.Count);
                }
                else
                {
                    sameHandRowJumpOccurrences.AddNull(bigram.Count);
                }

                if (firstKey.GetHandRoll(secondKey) is not Roll.None and Roll handRoll)
                {
                    handRollOccurrences.Add((firstKey.Hand, handRoll), bigram.Count);
                }
                else
                {
                    handRollOccurrences.AddNull(bigram.Count);
                }

                if (firstKey.GetFingerMotion(secondKey) is not Motion.None and Motion fingerMotion)
                {
                    fingerMotionOccurrences.Add((firstKey.Finger, fingerMotion), bigram.Count);
                }
                else
                {
                    fingerMotionOccurrences.AddNull(bigram.Count);
                }
            }

            var sameFingerBigramAnalysis = new OccurrenceAnalysis<Finger>(sameFingerBigramOccurrences);
            System.Console.WriteLine($"Same finger bigram: {sameFingerBigramAnalysis.Sum(x => x.Percentage):P3}");
            foreach (var item in sameFingerBigramAnalysis.OrderByDescending(x => x.Percentage))
            {
                System.Console.WriteLine($"{item.Percentage,12:P3}  {item.Value}");
            }

            sameFingerBigramHandOccurrences = sameFingerBigramOccurrences.Group(x => x.Value.GetHand());
            var sameFingerBigramHandAnalysis = new OccurrenceAnalysis<Hand>(sameFingerBigramHandOccurrences);
            System.Console.WriteLine($"Same finger hand bigram: {sameFingerBigramHandAnalysis.Sum(x => x.Percentage):P3}");
            foreach (var item in sameFingerBigramHandAnalysis.OrderByDescending(x => x.Percentage))
            {
                System.Console.WriteLine($"{item.Percentage,12:P3}  {item.Value}");
            }

            System.Console.WriteLine();

            var neighborFingerBigramAnalysis = new OccurrenceAnalysis<(Finger, Finger)>(neighborFingerBigramOccurrences);
            System.Console.WriteLine($"Neighbor finger bigram: {neighborFingerBigramAnalysis.Sum(x => x.Percentage):P3}");
            foreach (var item in neighborFingerBigramAnalysis.OrderByDescending(x => x.Percentage))
            {
                System.Console.WriteLine($"{item.Percentage,12:P3}  {item.Value}");
            }

            neighborFingerBigramHandOccurrences = neighborFingerBigramOccurrences.Group(x => x.Value.firstFinger.GetHand());
            var neighborFingerBigramHandAnalysis = new OccurrenceAnalysis<Hand>(neighborFingerBigramHandOccurrences);
            System.Console.WriteLine($"Neighbor finger hand bigram: {neighborFingerBigramHandAnalysis.Sum(x => x.Percentage):P3}");
            foreach (var item in neighborFingerBigramHandAnalysis.OrderByDescending(x => x.Percentage))
            {
                System.Console.WriteLine($"{item.Percentage,12:P3}  {item.Value}");
            }

            System.Console.WriteLine();

            var handRollAnalysis = new OccurrenceAnalysis<(Hand, Roll)>(handRollOccurrences);
            System.Console.WriteLine($"Hand roll: {handRollAnalysis.Sum(x => x.Percentage):P3}");
            foreach (var item in handRollAnalysis.OrderByDescending(x => x.Percentage))
            {
                System.Console.WriteLine($"{item.Percentage,12:P3}  {item.Value}");
            }

            rollOccurrences = handRollOccurrences.Group(x => x.Value.roll);
            var rollAnalysis = new OccurrenceAnalysis<Roll>(rollOccurrences);
            System.Console.WriteLine($"Roll: {rollAnalysis.Sum(x => x.Percentage):P3}");
            foreach (var item in rollAnalysis.OrderByDescending(x => x.Percentage))
            {
                System.Console.WriteLine($"{item.Percentage,12:P3}  {item.Value}");
            }

            System.Console.WriteLine();

            var handAlternationAnalysis = new OccurrenceAnalysis<bool>(handAlternationOccurrences);
            System.Console.WriteLine($"Hand alternation: {(handAlternationAnalysis.TryGet(true, out var alternation) ? alternation.Percentage : 0):P3}");

            System.Console.WriteLine();

            var sameHandRowJumpAnalysis = new OccurrenceAnalysis<(int, int)>(sameHandRowJumpOccurrences);
            System.Console.WriteLine($"Same hand row jump (from, to)");
            foreach (var item in sameHandRowJumpAnalysis.OrderByDescending(x => x.Percentage))
            {
                System.Console.WriteLine($"{item.Percentage,12:P3}  {item.Value}");
            }

            sameHandRowJumpDistanceOccurrences = sameHandRowJumpOccurrences.Group(x => Math.Abs(x.Value.firstRow - x.Value.secondRow));
            var sameHandRowJumpDistanceAnalysis = new OccurrenceAnalysis<int>(sameHandRowJumpDistanceOccurrences);
            System.Console.WriteLine($"Same hand row jump (distance)");
            foreach (var item in sameHandRowJumpDistanceAnalysis.OrderByDescending(x => x.Percentage))
            {
                System.Console.WriteLine($"{item.Percentage,12:P3}  {item.Value}");
            }

            System.Console.WriteLine();

            var fingerMotionAnalysis = new OccurrenceAnalysis<(Finger, Motion)>(fingerMotionOccurrences);
            System.Console.WriteLine($"Finger motion: {fingerMotionAnalysis.Sum(x => x.Percentage):P3}");
            foreach (var item in fingerMotionAnalysis.OrderByDescending(x => x.Percentage))
            {
                System.Console.WriteLine($"{item.Percentage,12:P3}  {item.Value}");
            }

            handMotionOccurrences = fingerMotionOccurrences.Group(x => (x.Value.finger.GetHand(), x.Value.motion));
            var handMotionAnalysis = new OccurrenceAnalysis<(Hand, Motion)>(handMotionOccurrences);
            System.Console.WriteLine($"Hand motion: {handMotionAnalysis.Sum(x => x.Percentage):P3}");
            foreach (var item in handMotionAnalysis.OrderByDescending(x => x.Percentage))
            {
                System.Console.WriteLine($"{item.Percentage,12:P3}  {item.Value}");
            }

            motionOccurrences = fingerMotionOccurrences.Group(x => x.Value.motion);
            var motionAnalysis = new OccurrenceAnalysis<Motion>(motionOccurrences);
            System.Console.WriteLine($"Motion: {motionAnalysis.Sum(x => x.Percentage):P3}");
            foreach (var item in motionAnalysis.OrderByDescending(x => x.Percentage))
            {
                System.Console.WriteLine($"{item.Percentage,12:P3}  {item.Value}");
            }

            System.Console.WriteLine();
        }
    }
}
