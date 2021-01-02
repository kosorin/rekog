//using Rekog.Core.Corpora;
//using Rekog.Core.Extensions;
//using System;
//using System.Linq;

//namespace Rekog.Core.Layouts.Analyzers
//{
//    public class NgramAnalyzer
//    {
//        public void Analyze(CorpusAnalysis corpusAnalysis, Layout layout)
//        {
//            var sameHandRowJumpDistanceOccurrences = new OccurrenceCollection<int>();
//            var handMotionOccurrences = new OccurrenceCollection<(Hand hand, Motion motion)>();
//            var sameFingerBigramHandOccurrences = new OccurrenceCollection<Hand>();
//            var neighborFingerBigramHandOccurrences = new OccurrenceCollection<Hand>();
//            var rollOccurrences = new OccurrenceCollection<Roll>();
//            var motionOccurrences = new OccurrenceCollection<Motion>();

//            var sameFingerBigramOccurrences = new OccurrenceCollection<Finger>();
//            var neighborFingerBigramOccurrences = new OccurrenceCollection<(Finger firstFinger, Finger secondFinger)>();
//            var sameFingerMotionOccurrences = new OccurrenceCollection<(Finger finger, Motion motion)>();
//            var sameHandRollOccurrences = new OccurrenceCollection<(Hand hand, Roll roll)>();
//            var sameHandRowJumpOccurrences = new OccurrenceCollection<(int firstRow, int secondRow)>();


//            var sameFingerBigramAnalysis = new OccurrenceAnalysis<Finger>(sameFingerBigramOccurrences);
//            System.Console.WriteLine($"Same finger bigram: {sameFingerBigramAnalysis.Sum(x => x.Percentage):P3}");
//            foreach (var item in sameFingerBigramAnalysis.OrderByDescending(x => x.Percentage))
//            {
//                System.Console.WriteLine($"{item.Percentage,12:P3}  {item.Value}");
//            }

//            sameFingerBigramHandOccurrences = sameFingerBigramOccurrences.Group(x => x.Value.GetHand());
//            var sameFingerBigramHandAnalysis = new OccurrenceAnalysis<Hand>(sameFingerBigramHandOccurrences);
//            System.Console.WriteLine($"Same finger hand bigram: {sameFingerBigramHandAnalysis.Sum(x => x.Percentage):P3}");
//            foreach (var item in sameFingerBigramHandAnalysis.OrderByDescending(x => x.Percentage))
//            {
//                System.Console.WriteLine($"{item.Percentage,12:P3}  {item.Value}");
//            }

//            System.Console.WriteLine();

//            var neighborFingerBigramAnalysis = new OccurrenceAnalysis<(Finger, Finger)>(neighborFingerBigramOccurrences);
//            System.Console.WriteLine($"Neighbor finger bigram: {neighborFingerBigramAnalysis.Sum(x => x.Percentage):P3}");
//            foreach (var item in neighborFingerBigramAnalysis.OrderByDescending(x => x.Percentage))
//            {
//                System.Console.WriteLine($"{item.Percentage,12:P3}  {item.Value}");
//            }

//            neighborFingerBigramHandOccurrences = neighborFingerBigramOccurrences.Group(x => x.Value.firstFinger.GetHand());
//            var neighborFingerBigramHandAnalysis = new OccurrenceAnalysis<Hand>(neighborFingerBigramHandOccurrences);
//            System.Console.WriteLine($"Neighbor finger hand bigram: {neighborFingerBigramHandAnalysis.Sum(x => x.Percentage):P3}");
//            foreach (var item in neighborFingerBigramHandAnalysis.OrderByDescending(x => x.Percentage))
//            {
//                System.Console.WriteLine($"{item.Percentage,12:P3}  {item.Value}");
//            }

//            System.Console.WriteLine();

//            var handRollAnalysis = new OccurrenceAnalysis<(Hand, Roll)>(sameHandRollOccurrences);
//            System.Console.WriteLine($"Hand roll: {handRollAnalysis.Sum(x => x.Percentage):P3}");
//            foreach (var item in handRollAnalysis.OrderByDescending(x => x.Percentage))
//            {
//                System.Console.WriteLine($"{item.Percentage,12:P3}  {item.Value}");
//            }

//            rollOccurrences = sameHandRollOccurrences.Group(x => x.Value.roll);
//            var rollAnalysis = new OccurrenceAnalysis<Roll>(rollOccurrences);
//            System.Console.WriteLine($"Roll: {rollAnalysis.Sum(x => x.Percentage):P3}");
//            foreach (var item in rollAnalysis.OrderByDescending(x => x.Percentage))
//            {
//                System.Console.WriteLine($"{item.Percentage,12:P3}  {item.Value}");
//            }

//            System.Console.WriteLine();


//            var sameHandRowJumpAnalysis = new OccurrenceAnalysis<(int, int)>(sameHandRowJumpOccurrences);
//            System.Console.WriteLine($"Same hand row jump (from, to)");
//            foreach (var item in sameHandRowJumpAnalysis.OrderByDescending(x => x.Percentage))
//            {
//                System.Console.WriteLine($"{item.Percentage,12:P3}  {item.Value}");
//            }

//            sameHandRowJumpDistanceOccurrences = sameHandRowJumpOccurrences.Group(x => Math.Abs(x.Value.firstRow - x.Value.secondRow));
//            var sameHandRowJumpDistanceAnalysis = new OccurrenceAnalysis<int>(sameHandRowJumpDistanceOccurrences);
//            System.Console.WriteLine($"Same hand row jump (distance)");
//            foreach (var item in sameHandRowJumpDistanceAnalysis.OrderByDescending(x => x.Percentage))
//            {
//                System.Console.WriteLine($"{item.Percentage,12:P3}  {item.Value}");
//            }

//            System.Console.WriteLine();

//            var fingerMotionAnalysis = new OccurrenceAnalysis<(Finger, Motion)>(sameFingerMotionOccurrences);
//            System.Console.WriteLine($"Finger motion: {fingerMotionAnalysis.Sum(x => x.Percentage):P3}");
//            foreach (var item in fingerMotionAnalysis.OrderByDescending(x => x.Percentage))
//            {
//                System.Console.WriteLine($"{item.Percentage,12:P3}  {item.Value}");
//            }

//            handMotionOccurrences = sameFingerMotionOccurrences.Group(x => (x.Value.finger.GetHand(), x.Value.motion));
//            var handMotionAnalysis = new OccurrenceAnalysis<(Hand, Motion)>(handMotionOccurrences);
//            System.Console.WriteLine($"Hand motion: {handMotionAnalysis.Sum(x => x.Percentage):P3}");
//            foreach (var item in handMotionAnalysis.OrderByDescending(x => x.Percentage))
//            {
//                System.Console.WriteLine($"{item.Percentage,12:P3}  {item.Value}");
//            }

//            motionOccurrences = sameFingerMotionOccurrences.Group(x => x.Value.motion);
//            var motionAnalysis = new OccurrenceAnalysis<Motion>(motionOccurrences);
//            System.Console.WriteLine($"Motion: {motionAnalysis.Sum(x => x.Percentage):P3}");
//            foreach (var item in motionAnalysis.OrderByDescending(x => x.Percentage))
//            {
//                System.Console.WriteLine($"{item.Percentage,12:P3}  {item.Value}");
//            }

//            System.Console.WriteLine();
//        }
//    }
//}
