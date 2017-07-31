using System;
using System.Collections.Generic;
using System.Threading;

namespace Solution
{
    class Solution
    {
        static void Main(string[] args)
        {
            Console.WriteLine("I've solved this problem.");

        }
    }

    public interface IService
    {
        int Lifetime { get; set; }
        bool IsTimeExpired();
        void ResetTimeExpired();
    } 

    public abstract class AbstractService:IService
    {
        private int _lifetime=0;
        private DateTime _timeExpired;
        


        public int Lifetime
        {
            get { return _lifetime; }
            set
            {
                _lifetime = value;
                ResetTimeExpired();
            }
        }
        public bool IsTimeExpired() { return _lifetime == 0 ? false : _timeExpired < DateTime.Now; }
         
        
        public AbstractService()
        {
            ResetTimeExpired();
        }
        public void ResetTimeExpired() { _timeExpired = DateTime.Now.AddSeconds(_lifetime);}
        
        
    }
    public class JobService:AbstractService
    {

        public int Price { get; set; }
    }

    public class UserService : AbstractService
    {
                
        public int Age { get; set; }
    }

    public class ServiceRepository
    {
        private object _lock = new object();

        Dictionary<string, IService> _services = new Dictionary<string, IService>();
        private int _defaultServiceLifetime;

        public int CachedCount { get { return _services.Count; } }

        public ServiceRepository(int serviceLifetime)
        {
            _defaultServiceLifetime = serviceLifetime;
            Thread t = new Thread(CheckAndRemoveExpired);
            t.IsBackground = true;
            t.Start();
        }

        private void CheckAndRemoveExpired()
        {
            var expiredKeys = new List<string>();
            while (true)
            {
                lock (_lock)
                {
                    foreach (var item in _services)
                    {
                        var srv = item.Value;
                        if (srv.IsTimeExpired()) expiredKeys.Add(item.Key);

                    }

                    foreach (var key in expiredKeys)
                    {
                        _services.Remove(key);
                    }
                }
                expiredKeys.Clear();
            }
        }


        public T GetService<T>(string key) where T :IService, new()  {
            
            lock (_lock)
            {
                if (_services.ContainsKey(key))
                {
                    var s = _services[key];
                    if (!s.IsTimeExpired()) {
                        s.ResetTimeExpired();
                        return (T)s;
                    };
                    _services.Remove(key);
                }

            }
            var srv = new T();
            srv.Lifetime = _defaultServiceLifetime;
            lock (_lock)
            {
                _services.Add(key, srv);
            }
            return srv;

        }

    }

    public class ContextRepository
    {
        ServiceRepository _repository;

        

        public ContextRepository(int defaultLifetime)
        {
            _repository = new ServiceRepository(defaultLifetime);
        }

        public UserService GetUser(string key)
        {
            return _repository.GetService<UserService>(key);

        }


        public JobService GetJob(string key)
        {
            return _repository.GetService<JobService>(key);

        }

    }

    #region Some Tests
    //[TestClass]
    //public class UnitTest1
    //{
    //    [TestMethod]
    //    public void TestUser()
    //    {
    //        ContextRepository repository = new ContextRepository(1);

    //        UserService user = repository.GetUser("John");

    //        Assert.AreEqual(0, user.Age);

    //        user.Age = 20;
    //        Assert.AreEqual(20, user.Age);

    //        user = repository.GetUser("John");
    //        Assert.AreEqual(20, user.Age);


    //    }

    //    [TestMethod]
    //    public void TestUser2()
    //    {
    //        var user = new UserService();

    //        Assert.IsFalse(user.IsTimeExpired());




    //    }

    //    [TestMethod]
    //    public void TestUserExpired()
    //    {
    //        ContextRepository repository = new ContextRepository(1);

    //        UserService user = repository.GetUser("John");

    //        user.Age = 20;
    //        Thread.Sleep(2000);
    //        user = repository.GetUser("John");
    //        Assert.AreEqual(0, user.Age);

    //    }

    //    [TestMethod]
    //    public void TestUserNotExpired()
    //    {
    //        ContextRepository repository = new ContextRepository(1);

    //        UserService user = repository.GetUser("John");

    //        user.Age = 20;
    //        Thread.Sleep(500);
    //        user = repository.GetUser("John");
    //        Assert.AreEqual(20, user.Age);

    //    }







    //    [TestMethod]
    //    public void TestJobNeverExpired()
    //    {
    //        ServiceRepository repository = new ServiceRepository(1);

    //        JobService job = repository.GetService<JobService>("Designing");

    //        job.Price = 1000;
    //        job.Lifetime = 0;
    //        Thread.Sleep(2000);
    //        job = repository.GetService<JobService>("Designing");
    //        Assert.AreEqual(1000, job.Price);


    //    }






    //    [TestMethod]
    //    public void TestJobNotExpired2()
    //    {
    //        ContextRepository repository = new ContextRepository(1);

    //        JobService job = repository.GetJob("Designing");

    //        job.Price = 1000;


    //        Thread.Sleep(500);
    //        job = repository.GetJob("Designing");
    //        Thread.Sleep(500);
    //        job = repository.GetJob("Designing");
    //        Thread.Sleep(500);
    //        job = repository.GetJob("Designing");
    //        Thread.Sleep(500);
    //        job = repository.GetJob("Designing");



    //        Assert.AreEqual(1000, job.Price);


    //    }

    //    [TestMethod]
    //    public void TestChangeLifetime()
    //    {
    //        ContextRepository repository = new ContextRepository(1);

    //        JobService job = repository.GetJob("Designing");

    //        job.Price = 1000;
    //        job.Lifetime = 5;

    //        Thread.Sleep(2000);

    //        job = repository.GetJob("Designing");
    //        Assert.AreEqual(1000, job.Price);


    //    }

    //    [TestMethod]
    //    public void TestRepository()
    //    {
    //        ServiceRepository repository = new ServiceRepository(5);

    //        Assert.AreEqual(0, repository.CachedCount);

    //        JobService job = repository.GetService<JobService>("Designing");
    //        Thread.Sleep(3000);

    //        job = repository.GetService<JobService>("Developing");
    //        Assert.AreEqual(2, repository.CachedCount);

    //        Thread.Sleep(3000);

    //        Assert.AreEqual(1, repository.CachedCount);

    //        Thread.Sleep(3000);

    //        Assert.AreEqual(0, repository.CachedCount);


    //    }
    //}
    #endregion


}