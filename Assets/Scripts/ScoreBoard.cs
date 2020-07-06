namespace LazySamurai.RadialShooter
{
    public class ScoreBoard
    {
        private readonly ViewController _viewController;

        public int Value { get; private set; }

        public ScoreBoard(ViewController viewController)
        {
            _viewController = viewController;
        }

        public void AddScore()
        {
            Value++;
            _viewController.SetScore(Value);
        }

        public void Reset()
        {
            Value = 0;
            _viewController.SetScore(0);
        }
    }
}