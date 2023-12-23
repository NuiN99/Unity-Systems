namespace NuiN.Movement
{
    public class MovementConstraints
    {
        public readonly bool canMove;
        public readonly bool canRotate;

        public MovementConstraints(bool canMove, bool canRotate)
        {
            this.canMove = canMove;
            this.canRotate = canRotate;
        }
    }
}