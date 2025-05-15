/// <summary>
/// Represents the different parameters that the animators will use.
/// </summary>
public static partial class Animators
{
    /// <summary>
    /// Represents the different parameters that the player animator can use.
    /// </summary>
    public static class Player
    {
        /// <summary>
        /// The speed parameter that is used to transition between idle and run.
        /// </summary>
        public const string Speed = nameof(Speed);

        /// <summary>
        /// The jump parameter that is used to specify when the player is jumping.
        /// </summary>
        public const string IsJumping = nameof(IsJumping);

        /// <summary>
        /// The crouch parameter that is used to specify when the player is crouching.
        /// </summary>
        public const string IsCrouching = nameof(IsCrouching);

        /// <summary>
        /// The roll parameter that is used to specify when the player is rolling.
        /// </summary>
        public const string IsRolling = nameof(IsRolling);

        /// <summary>
        /// The fall parameter that is used to specify when the player is falling.
        /// </summary>
        public const string IsFalling = nameof(IsFalling);

        /// <summary>
        /// The attack parameter that is used to specify when the player is attacking.
        /// </summary>
        public const string IsAttacking = nameof(IsAttacking);

        /// <summary>
        /// The attack parameter that is used to specify when the player is performing an elemental attack.
        /// </summary>
        public static string IsElemAttacking = nameof(IsElemAttacking);

        /// <summary>
        /// The attack parameter that is used to specify which element the player currently has selected.
        /// </summary>
        public static string CurrElem = nameof(CurrElem);
    }
    public static class Spell
    {
        /// <summary>
        /// The element parameter that is used to decide which animation to play.
        /// </summary>
        public const string Element = nameof(Element);
    }
}