using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solution;
using System.Threading;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestUser()
        {
            ContextRepository repository = new ContextRepository(1);

            UserService user = repository.GetUser("John");

            Assert.AreEqual(0, user.Age);

            user.Age = 20;
            Assert.AreEqual(20, user.Age);

            user = repository.GetUser("John");
            Assert.AreEqual(20, user.Age);


        }

        [TestMethod]
        public void TestUser2()
        {
            var user = new UserService();

            Assert.IsFalse(user.IsTimeExpired());




        }

        [TestMethod]
        public void TestUserExpired()
        {
            ContextRepository repository = new ContextRepository(1);

            UserService user = repository.GetUser("John");

            user.Age = 20;
            Thread.Sleep(2000);
            user = repository.GetUser("John");
            Assert.AreEqual(0, user.Age);

        }

        [TestMethod]
        public void TestUserNotExpired()
        {
            ContextRepository repository = new ContextRepository(1);

            UserService user = repository.GetUser("John");

            user.Age = 20;
            Thread.Sleep(500);
            user = repository.GetUser("John");
            Assert.AreEqual(20, user.Age);

        }







        [TestMethod]
        public void TestJobNeverExpired()
        {
            ServiceRepository repository = new ServiceRepository(1);

            JobService job = repository.GetService<JobService>("Designing");

            job.Price = 1000;
            job.Lifetime = 0;
            Thread.Sleep(2000);
            job = repository.GetService<JobService>("Designing");
            Assert.AreEqual(1000, job.Price);


        }






        [TestMethod]
        public void TestJobNotExpired2()
        {
            ContextRepository repository = new ContextRepository(1);

            JobService job = repository.GetJob("Designing");

            job.Price = 1000;


            Thread.Sleep(500);
            job = repository.GetJob("Designing");
            Thread.Sleep(500);
            job = repository.GetJob("Designing");
            Thread.Sleep(500);
            job = repository.GetJob("Designing");
            Thread.Sleep(500);
            job = repository.GetJob("Designing");



            Assert.AreEqual(1000, job.Price);


        }

        [TestMethod]
        public void TestChangeLifetime()
        {
            ContextRepository repository = new ContextRepository(1);

            JobService job = repository.GetJob("Designing");

            job.Price = 1000;
            job.Lifetime = 5;

            Thread.Sleep(2000);

            job = repository.GetJob("Designing");
            Assert.AreEqual(1000, job.Price);


        }

        [TestMethod]
        public void TestRepository()
        {
            ServiceRepository repository = new ServiceRepository(5);

            Assert.AreEqual(0, repository.CachedCount);

            JobService job = repository.GetService<JobService>("Designing");
            Thread.Sleep(3000);

            job = repository.GetService<JobService>("Developing");
            Assert.AreEqual(2, repository.CachedCount);

            Thread.Sleep(3000);

            Assert.AreEqual(1, repository.CachedCount);

            Thread.Sleep(3000);

            Assert.AreEqual(0, repository.CachedCount);


        }
    }
}
