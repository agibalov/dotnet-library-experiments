﻿using NUnit.Framework;

namespace DapperExperiment.SingleTableTests
{
    public class Tests : AbstractDatabaseTest
    {
        private UserDAO _dao;

        [SetUp]
        public void ConstructUserDAO()
        {
            _dao = new UserDAO(DatabaseHelper);
            _dao.CreateSchema();
        }

        [Test]
        public void ThereAreNoUsersByDefault()
        {
            var userCount = _dao.GetUserCount();
            Assert.AreEqual(0, userCount);
        }

        [Test]
        public void CanCreateUser()
        {
            var user = _dao.CreateUser("loki2302");
            Assert.AreEqual(1, user.UserId);
            Assert.AreEqual("loki2302", user.UserName);
        }

        [Test]
        public void CanFetchSingleUser()
        {
            var user = _dao.CreateUser("loki2302");
            var fetchedUser = _dao.GetUser(user.UserId);
            Assert.AreEqual(user.UserId, fetchedUser.UserId);
            Assert.AreEqual(user.UserName, fetchedUser.UserName);
        }

        [Test]
        public void CanFetchMultipleUsers()
        {
            var user1 = _dao.CreateUser("loki2302_1");
            _dao.CreateUser("loki2302_2");
            var user3 = _dao.CreateUser("loki2302_3");

            var users = _dao.GetUsers(new[] { user1.UserId, user3.UserId });
            Assert.AreEqual(2, users.Count);
            
            var fetchedUser1 = users[0];
            Assert.AreEqual(user1.UserId, fetchedUser1.UserId);
            Assert.AreEqual(user1.UserName, fetchedUser1.UserName);

            var fetchedUser2 = users[1];
            Assert.AreEqual(user3.UserId, fetchedUser2.UserId);
            Assert.AreEqual(user3.UserName, fetchedUser2.UserName);
        }

        [Test]
        public void CanDeleteSingleUser()
        {
            _dao.CreateUser("loki2302_1");
            var userToDelete = _dao.CreateUser("loki2302_2");
            _dao.CreateUser("loki2302_3");

            var userCountBeforeDelete = _dao.GetUserCount();
            Assert.AreEqual(3, userCountBeforeDelete);

            _dao.DeleteUser(userToDelete.UserId);

            var userCountAfterDelete = _dao.GetUserCount();
            Assert.AreEqual(2, userCountAfterDelete);
        }

        [Test]
        public void CanDeleteMultipleUsers()
        {
            _dao.CreateUser("loki2302_1");
            var userToDelete1 = _dao.CreateUser("loki2302_2");
            var userToDelete2 = _dao.CreateUser("loki2302_3");

            var userCountBeforeDelete = _dao.GetUserCount();
            Assert.AreEqual(3, userCountBeforeDelete);

            _dao.DeleteUsers(new[]{ userToDelete1.UserId, userToDelete2.UserId });

            var userCountAfterDelete = _dao.GetUserCount();
            Assert.AreEqual(1, userCountAfterDelete);
        }

        [Test]
        public void CanChangeUserName()
        {
            var user = _dao.CreateUser("loki2302");
            Assert.AreEqual("loki2302", user.UserName);

            user = _dao.ChangeUserName(user.UserId, "qwerty");
            Assert.AreEqual("qwerty", user.UserName);
        }
    }
}
