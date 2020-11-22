using System.Collections.Generic;

namespace Rekog.Input.Options
{
    public abstract class CommandOptions : Input
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Config { get; set; }

        public string Output { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected override void FixSelf()
        {
            Config ??= string.Empty;
            Output ??= string.Empty;
        }

        protected override IEnumerable<Input> CollectChildren()
        {
            yield break;
        }
    }
}
