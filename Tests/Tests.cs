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

            Variable x = new(nameof(x));
            Variable y = new(nameof(y));
            Variable z = new(nameof(z));

            var s1 = P1.Invoke(123, 234);
            var s2 = P1.Invoke(x, 234);

            Assert.IsTrue(s1.Matches(s1));

            Assert.IsTrue(s1.Matches(s2));
            Assert.IsTrue(s2.Matches(s1));

            var s3 = P1.Invoke(x, x);
            var s4 = P1.Invoke(123, 123);

            Assert.IsTrue(s3.Matches(s4));
            Assert.IsTrue(s4.Matches(s3));

            var s5 = z.Invoke(123, 234);

            Assert.IsTrue(s1.Matches(s5));
            Assert.IsTrue(s5.Matches(s1));

            Assert.IsFalse(s1.Matches(s4));
            Assert.IsFalse(s1.Matches(s3));
            Assert.IsFalse(s3.Matches(s1));

            Assert.IsTrue(s3.Matches(s3));

            var s6 = P1.Invoke(y, y);

            Assert.IsTrue(s3.Matches(s6));
            Assert.IsTrue(s6.Matches(s3));
        }

        [Test]
        public void Test2()
        {
            Predicate P1 = new("Tests", nameof(P1), 3);
            Predicate P2 = new("Tests", nameof(P2), 2);
            Predicate P3 = new("Tests", nameof(P3), 2);

            Variable x = new(nameof(x));
            Variable y = new(nameof(y));
            Variable z = new(nameof(z));

            var s1 = P1.Invoke(123, P2.Invoke(234, 345), P2.Invoke(345, 456));
            var s2 = P1.Invoke(123, P2.Invoke(234, x), P2.Invoke(x, 456));
            var s3 = P1.Invoke(123, P2.Invoke(234, x), P2.Invoke(345, x));
            var s4 = P1.Invoke(123, x, P2.Invoke(345, 456));

            Assert.IsTrue(s1.IsGround);
            Assert.IsFalse(s2.IsGround);
            Assert.IsFalse(s3.IsGround);

            Assert.IsTrue(s1.Matches(s2));
            Assert.IsTrue(s2.Matches(s1));

            Assert.IsFalse(s1.Matches(s3));
            Assert.IsFalse(s3.Matches(s1));

            Assert.IsFalse(s2.Matches(s3));
            Assert.IsFalse(s3.Matches(s2));

            Assert.IsTrue(s1.Matches(s4));
            Assert.IsTrue(s4.Matches(s1));

            Assert.IsFalse(s3.Matches(s4));
            Assert.IsFalse(s4.Matches(s3));
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