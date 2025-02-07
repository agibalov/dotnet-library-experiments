﻿using System.Collections.Generic;
using System.Linq;
using Dapper;
using DapperExperiment.MultipleTablesTests.DAL;
using DapperExperiment.MultipleTablesTests.Service.DTO;

namespace DapperExperiment.MultipleTablesTests.Service
{
    public class BlogService
    {
        private readonly SqlCeDatabaseHelper _sqlCeDatabaseHelper;
        private readonly UserDAO _userDao;
        private readonly PostDAO _postDao;

        public BlogService(
            SqlCeDatabaseHelper sqlCeDatabaseHelper, 
            UserDAO userDao, 
            PostDAO postDao)
        {
            _sqlCeDatabaseHelper = sqlCeDatabaseHelper;
            _userDao = userDao;
            _postDao = postDao;
        }

        public void CreateSchema()
        {
            using (var connection = _sqlCeDatabaseHelper.MakeConnection())
            {
                connection.Execute(
                    "create table Users(" + 
                    "UserId int not null identity(1, 1) primary key, " +
                    "RowGuid uniqueidentifier not null, " +
                    "UserName nvarchar(256) not null)");

                connection.Execute(
                    "create table Posts(" +
                    "PostId int not null identity(1, 1) primary key, " +
                    "RowGuid uniqueidentifier not null, " +
                    "PostText nvarchar(256) not null, " + 
                    "UserId int not null references Users(UserId))");
            }
        }

        public UserDTO CreateUser(string userName)
        {
            using (var connection = _sqlCeDatabaseHelper.MakeConnection())
            {
                var userRow = _userDao.CreateUser(connection, userName);
                var postRows = _postDao.GetUserPosts(connection, userRow.UserId);
                var userDto = MakeUserDTO(userRow, postRows);
                return userDto;
            }
        }

        public UserDTO GetUser(int userId)
        {
            using (var connection = _sqlCeDatabaseHelper.MakeConnection())
            {
                var userRow = _userDao.GetUserOrThrow(connection, userId);
                var postRows = _postDao.GetUserPosts(connection, userId);
                var userDto = MakeUserDTO(userRow, postRows);
                return userDto;
            }
        }

        public IList<UserDTO> GetUsers(IList<int> userIds)
        {
            using (var connection = _sqlCeDatabaseHelper.MakeConnection())
            {
                var userRows = _userDao.GetUsersOrThrow(connection, userIds);
                var postRows = _postDao.GetPostsForManyUsers(connection, userIds);
                return MakeUserDTOs(userRows, postRows);
            }
        }

        public IList<UserDTO> GetAllUsers()
        {
            using (var connection = _sqlCeDatabaseHelper.MakeConnection())
            {
                var userRows = _userDao.GetAllUsers(connection);
                var userIds = userRows.Select(userRow => userRow.UserId).ToList();
                var postRows = _postDao.GetPostsForManyUsers(connection, userIds);
                return MakeUserDTOs(userRows, postRows);
            }
        }

        public int GetUserCount()
        {
            using (var connection = _sqlCeDatabaseHelper.MakeConnection())
            {
                var userCount = _userDao.GetUserCount(connection);
                return userCount;
            }
        }

        public UserDTO UpdateUser(int userId, string userName)
        {
            using (var connection = _sqlCeDatabaseHelper.MakeConnection())
            {
                var userRow = _userDao.UpdateUser(connection, userId, userName);
                var postRows = _postDao.GetUserPosts(connection, userId);
                return MakeUserDTO(userRow, postRows);
            }
        }

        public void DeleteUser(int userId)
        {
            using (var connection = _sqlCeDatabaseHelper.MakeConnection())
            {
                _userDao.GetUserOrThrow(connection, userId);

                var userPosts = _postDao.GetUserPosts(connection, userId);
                var postIds = userPosts.Select(post => post.PostId).ToList();
                _postDao.DeletePosts(connection, postIds);

                _userDao.DeleteUser(connection, userId);
            }
        }

        public void DeleteUsers(IList<int> userIds)
        {
            using (var connection = _sqlCeDatabaseHelper.MakeConnection())
            {
                _userDao.GetUsersOrThrow(connection, userIds);
                _userDao.DeleteUsers(connection, userIds);
            }
        }

        public PostDTO CreatePost(int userId, string postText)
        {
            using (var connection = _sqlCeDatabaseHelper.MakeConnection())
            {
                _userDao.GetUserOrThrow(connection, userId);
                var postRow = _postDao.CreatePost(connection, userId, postText);
                var postDto = MakePostDTO(postRow);
                return postDto;
            }
        }

        public PostDTO GetPost(int postId)
        {
            using (var connection = _sqlCeDatabaseHelper.MakeConnection())
            {
                var postRow = _postDao.GetPostOrThrow(connection, postId);
                var postDto = MakePostDTO(postRow);
                return postDto;
            }
        }

        public IList<PostDTO> GetPosts(IList<int> postIds)
        {
            using (var connection = _sqlCeDatabaseHelper.MakeConnection())
            {
                var postRows = _postDao.GetPostsOrThrow(connection, postIds);
                var postDtos = MakePostDTOs(postRows);
                return postDtos;
            }
        }

        public IList<PostDTO> GetAllPosts()
        {
            using (var connection = _sqlCeDatabaseHelper.MakeConnection())
            {
                var postRows = _postDao.GetAllPosts(connection);
                var postDtos = MakePostDTOs(postRows);
                return postDtos;
            }
        }

        public PostDTO UpdatePost(int postId, string postText)
        {
            using (var connection = _sqlCeDatabaseHelper.MakeConnection())
            {
                var postRow = _postDao.UpdatePost(connection, postId, postText);
                return MakePostDTO(postRow);
            }
        }

        public int GetPostCount()
        {
            using (var connection = _sqlCeDatabaseHelper.MakeConnection())
            {
                var postCount = _postDao.GetPostCount(connection);
                return postCount;
            }
        }

        public void DeletePost(int postId)
        {
            using (var connection = _sqlCeDatabaseHelper.MakeConnection())
            {
                _postDao.GetPostOrThrow(connection, postId);
                _postDao.DeletePost(connection, postId);
            }
        }

        public void DeletePosts(IList<int> postIds)
        {
            using (var connection = _sqlCeDatabaseHelper.MakeConnection())
            {
                _postDao.GetPostsOrThrow(connection, postIds);
                _postDao.DeletePosts(connection, postIds);
            }
        }

        public IList<UserAndPostCountDTO> GetUsersWithPostCount()
        {
            using (var connection = _sqlCeDatabaseHelper.MakeConnection())
            {
                var userWithPostCountRows = _userDao.GetUsersWithPostCount(connection);
                return (from userAndPostCountRow in userWithPostCountRows
                        select new UserAndPostCountDTO
                            {
                                User = new BriefUserDTO { UserId = userAndPostCountRow.UserId, UserName = userAndPostCountRow.UserName },
                                PostCount = userAndPostCountRow.PostCount
                            }).ToList();
            }
        }

        private static UserDTO MakeUserDTO(UserRow userRow, IList<PostRow> postRows)
        {
            return new UserDTO
            {
                UserId = userRow.UserId,
                UserName = userRow.UserName,
                Posts = (from postRow in postRows
                         where postRow.UserId == userRow.UserId
                         select MakePostDTO(postRow)).ToList()
            };
        }

        private static IList<UserDTO> MakeUserDTOs(IList<UserRow> userRows, IList<PostRow> postRows)
        {
            return (from userRow in userRows
                    select MakeUserDTO(userRow, postRows)).ToList();
        }

        private static PostDTO MakePostDTO(PostRow postRow)
        {
            return new PostDTO
                {
                    PostId = postRow.PostId,
                    PostText = postRow.PostText,
                    User = new BriefUserDTO
                        {
                            UserId = postRow.UserId,
                            UserName = postRow.UserName
                        }
                };
        }

        private static IList<PostDTO> MakePostDTOs(IList<PostRow> postRows)
        {
            return (from postRow in postRows
                    select MakePostDTO(postRow)).ToList();
        }
    }
}