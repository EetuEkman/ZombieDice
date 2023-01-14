namespace ZombieDiceLibrary
{
    public class Facing
    {
        public Facing(ZombieDieFacings value) {
            switch(value)
            {
                case ZombieDieFacings.Brain:
                    this.Value = ZombieDieFacings.Brain;
                    this.Image = FacingImages.Brain;
                    break;
                case ZombieDieFacings.Footprints:
                    this.Value = ZombieDieFacings.Footprints;
                    this.Image = FacingImages.Footprints;
                    break;
                case ZombieDieFacings.Shotgun:
                    this.Value = ZombieDieFacings.Shotgun;
                    this.Image = FacingImages.Shotgun;
                    break;
                default:
                    throw new ArgumentException("");
            }
        }

        public ZombieDieFacings Value;

        public string Image;
    }
}