using AI.Epistemology;
using AI.Epistemology.Reasoning;
using System.Collections;
using System.Collections.Graphs;
using System.Linq.Expressions;

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

    public class StatementCollection
    {
        public class Prototype : IStatementCollection
        {
            public Prototype(IEnumerable<Statement> statements)
            {
                m_data = new List<Statement>(statements);
                m_queryable = m_data.AsQueryable();
            }

            public IEdge<IStatementCollection, IStatementCollection, IReasoner>? Binding => throw new NotImplementedException();

            private readonly List<Statement> m_data;
            private readonly IQueryable<Statement> m_queryable;

            public bool IsReadOnly => false;

            public Type ElementType => m_queryable.ElementType;

            public Expression Expression => m_queryable.Expression;

            public IQueryProvider Provider => m_queryable.Provider;

            public bool IsValid => throw new NotImplementedException();

            public IStatementCollection Clone()
            {
                throw new NotImplementedException();
            }

            public bool Contains(Statement statement)
            {
                return m_data.Contains(statement);
            }

            public bool Contains(Statement statement, out IEnumerable? derivations)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<Statement> GetEnumerator()
            {
                return m_queryable.GetEnumerator();
            }

            public void Update(IEnumerable<Statement> removals, IEnumerable<Statement> additions)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return m_queryable.GetEnumerator();
            }
        }

        [Test]
        public void Test1()
        {
            Predicate Knows = new("Tests", nameof(Knows), 2);

            var Alice = "Alice";
            var Bob = "Bob";
            var Charlie = "Charlie";
            var Douglas = "Douglas";
            var Edward = "Edward";
            var Frank = "Frank";
            var Xavier = "Xavier";
            var Yelena = "Yelena";

            Variable X = new(nameof(X));
            Variable Y = new(nameof(Y));
            Variable Z = new(nameof(Z));
            Variable W = new(nameof(W));

            IStatementCollection KB = new Prototype(new Statement[]
            {
                Knows.Invoke(Alice, Bob),
                Knows.Invoke(Bob, Charlie),
                Knows.Invoke(Charlie, Douglas),
                Knows.Invoke(Douglas, Edward),
                Knows.Invoke(Edward, Frank),

                Knows.Invoke(Alice, Xavier),
                Knows.Invoke(Xavier, Yelena),
                Knows.Invoke(Yelena, Douglas)
            });

            var query = new Statement[] { Knows.Invoke(Alice, X) };

            foreach (var result in KB.Query(query))
            {
                Console.WriteLine(result[X]);
                Console.WriteLine();
            }
        }

        [Test]
        public void Test2()
        {
            Predicate Knows = new("Tests", nameof(Knows), 2);

            var Alice = "Alice";
            var Bob = "Bob";
            var Charlie = "Charlie";
            var Douglas = "Douglas";
            var Edward = "Edward";
            var Frank = "Frank";
            var Xavier = "Xavier";
            var Yelena = "Yelena";

            Variable X = new(nameof(X));
            Variable Y = new(nameof(Y));
            Variable Z = new(nameof(Z));
            Variable W = new(nameof(W));

            IStatementCollection KB = new Prototype(new Statement[]
            {
                Knows.Invoke(Alice, Bob),
                Knows.Invoke(Bob, Charlie),
                Knows.Invoke(Charlie, Douglas),
                Knows.Invoke(Douglas, Edward),
                Knows.Invoke(Edward, Frank),

                Knows.Invoke(Alice, Xavier),
                Knows.Invoke(Xavier, Yelena),
                Knows.Invoke(Yelena, Douglas)
            });

            var query = new Statement[] { Knows.Invoke(Alice, X), Knows.Invoke(X, Y), Knows.Invoke(Y, Z), Knows.Invoke(Z, W), Knows.Invoke(W, Frank) };

            foreach (var result in KB.Query(query))
            {
                Console.WriteLine(result[X]);
                Console.WriteLine(result[Y]);
                Console.WriteLine(result[Z]);
                Console.WriteLine(result[W]);
                Console.WriteLine();
            }
        }

        [Test]
        public void Test3()
        {
            Predicate Knows = new("Tests", nameof(Knows), 2);

            var Alice = "Alice";
            var Bob = "Bob";
            var Charlie = "Charlie";
            var Douglas = "Douglas";
            var Edward = "Edward";
            var Frank = "Frank";
            var Hubert = "Hubert";
            var Xavier = "Xavier";
            var Yelena = "Yelena";
            var Wilma = "Wilma";

            Variable X = new(nameof(X));
            Variable Y = new(nameof(Y));
            Variable Z = new(nameof(Z));
            Variable W = new(nameof(W));

            IStatementCollection KB = new Prototype(new Statement[]
            {
                Knows.Invoke(Yelena, Douglas),
                Knows.Invoke(Charlie, Douglas),
                Knows.Invoke(Douglas, Edward),

                Knows.Invoke(Alice, Bob),
                Knows.Invoke(Bob, Charlie),
                Knows.Invoke(Edward, Frank),

                Knows.Invoke(Alice, Xavier),
                Knows.Invoke(Xavier, Yelena),

                Knows.Invoke(Alice, Wilma),
                Knows.Invoke(Wilma, Douglas),
                Knows.Invoke(Douglas, Charlie),
                Knows.Invoke(Charlie, Hubert),
                Knows.Invoke(Hubert, Frank),
            });

            var query = new Statement[] { Knows.Invoke(Alice, X), Knows.Invoke(X, Y), Knows.Invoke(Y, Z), Knows.Invoke(Z, W), Knows.Invoke(W, Frank) };

            foreach (var result in KB.Query(query))
            {
                try
                {
                    Console.WriteLine(result[X]);
                    Console.WriteLine(result[Y]);
                    Console.WriteLine(result[Z]);
                    Console.WriteLine(result[W]);
                }
                catch
                {
                    Console.WriteLine("Error");
                }
                Console.WriteLine();
            }
        }

        [Test]
        public void Test4()
        {
            Predicate SawUsing = new("Tests", nameof(SawUsing), 3);

            var Alice = "Alice";
            var Bob = "Bob";

            var Mercury = "Mercury";
            var Moon = "Moon";

            var Binoculars = "Binoculars";
            var Telescope = "Telescope";

            Variable X = new(nameof(X));
            Variable Y = new(nameof(Y));

            IStatementCollection KB = new Prototype(new Statement[]
            {
                SawUsing.Invoke(Alice, Moon, Binoculars),
                SawUsing.Invoke(Bob, Mercury, Binoculars),
                SawUsing.Invoke(Bob, Mercury, Telescope)
            });

            var query = new Statement[] { SawUsing.Invoke(X, Y, Binoculars) };

            foreach(var result in KB.Query(query))
            {
                Console.WriteLine(result[X]);
                Console.WriteLine(result[Y]);
                Console.WriteLine();
            }
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