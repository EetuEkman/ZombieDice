namespace ZombieDiceLibrary
{
    /// <summary>
    /// Represens a zombie die with id, facing, sides and colors.
    /// </summary>
    public class Die
    {
        public Die(int id, Colors color)
        {
            Id = id;
            Color = color;

            switch (color)
            {
                case Colors.Green:
                    this.Facings = new List<Facing>
                        {
                            new Facing(ZombieDieFacings.Brain),
                            new Facing(ZombieDieFacings.Brain),
                            new Facing(ZombieDieFacings.Brain),
                            new Facing(ZombieDieFacings.Footprints),
                            new Facing(ZombieDieFacings.Footprints),
                            new Facing(ZombieDieFacings.Shotgun),
                        };
                    break;
                case Colors.Yellow:
                    this.Facings = new List<Facing>
                        {
                            new Facing(ZombieDieFacings.Brain),
                            new Facing(ZombieDieFacings.Brain),
                            new Facing(ZombieDieFacings.Footprints),
                            new Facing(ZombieDieFacings.Footprints),
                            new Facing(ZombieDieFacings.Shotgun),
                            new Facing(ZombieDieFacings.Shotgun),
                        };
                    break;
                case Colors.Red:
                    this.Facings = new List<Facing>
                        {
                            new Facing(ZombieDieFacings.Brain),
                            new Facing(ZombieDieFacings.Footprints),
                            new Facing(ZombieDieFacings.Footprints),
                            new Facing(ZombieDieFacings.Shotgun),
                            new Facing(ZombieDieFacings.Shotgun),
                            new Facing(ZombieDieFacings.Shotgun),
                        };
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        public int Id { get; set; }
        /// <summary>
        /// Represents the different facings of the die.
        /// </summary>
        public List<Facing> Facings { get; set; } = new List<Facing>();
        /// <summary>
        /// Represents the upper surface of the resting die, usually denoting roll value.
        /// </summary>
        public Facing? Facing { get; set; }
        /// <summary>
        /// Represents the color of the dice.
        /// </summary>
        public Colors Color { get; set; }

        /// <summary>
        /// Roll the die.
        /// </summary>
        /// <returns>New Die with current die state and the randomized facing.</returns>
        public Die Roll()
        {
            var die = new Die(Id, Color);

            if (Facings.Count == 0)
            {
                return die;
            }

            die.Facing = Facings[Random.Shared.Next(0, Facings.Count)];

            return die;
        }
    }
}
