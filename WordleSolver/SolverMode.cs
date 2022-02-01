
// (c) 2021 Kazuki KOHZUKI

using System.ComponentModel;

namespace Wordle
{
    internal enum SolverMode
    {
        [Description("Hard mode, use simplified score")]
        HardMode    = 0x00,

        [Description("Hard mode, use entropy")]
        UseEntropy  = 0x01,

        [Description("Non-hard mode, use simplified score")]
        NonHardMode = 0x10,

        [Description("Non-hard mode, use weighted entropy")]
        NonHardUseEntropy = 0x11,
    } // internal enum SolverMode
} // namespace Wordle
