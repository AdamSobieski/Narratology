using AI.Epistemology;

namespace Tests
{
    public class PredicateCalculus
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Predicate P1 = new("Tests", nameof(P1), 2);
            Predicate P2 = new("Tests", nameof(P2), 2);
            Predicate P3 = new("Tests", nameof(P3), 2);

            Variable x = new();
            Variable y = new();
            Variable z = new();

            var s1 = P1.Invoke(x, y);
        }
    }

    public class StateTrajectoryConstraints
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {

        }
    }
}