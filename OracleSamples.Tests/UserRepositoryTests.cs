using Microsoft.VisualStudio.TestTools.UnitTesting;
using OracleSamples.Data;
using OracleSamples.Model;

namespace OracleSamples.Tests
{
    [TestClass]
    public class UserRepositoryTests
    {
        [TestMethod]
        public void CountTest()
        {
            var repo = new UserRepository();
            var data = repo.Count_Sql();
        }

        [TestMethod]
        public void Count2Test()
        {
            var repo = new UserRepository();
            var data = repo.Count_Procedure();
        }

        [TestMethod]
        public void GetAll_DataTableTest()
        {
            var repo = new UserRepository();
            var data = repo.GetAll_DataTable();
        }

        [TestMethod]
        public void GetAll_ReaderToListTest()
        {
            var repo = new UserRepository();
            var data = repo.GetAll_ReaderToList();
        }

        [TestMethod]
        public void GetAll_LinqToListTest()
        {
            var repo = new UserRepository();
            var data = repo.GetAll_DataTableLinqToList();
        }

        [TestMethod]
        public void GetAll_ReaderLinqToListTest()
        {
            var repo = new UserRepository();
            var data = repo.GetAll_ReaderLinqToList();
        }

        [TestMethod]
        public void GetAll_ReaderLinqToListWithOrdinalTest()
        {
            var repo = new UserRepository();
            var data = repo.GetAll_ReaderLinqToListWithOrdinal();
        }

        [TestMethod]
        public void GetByIdTest()
        {
            var repo = new UserRepository();
            var data = repo.GetById("jholland");
        }

        [TestMethod]
        public void GetByIdsTest()
        {
            var repo = new UserRepository();
            var data = repo.GetByIds(new[] { "jholland", "msmith" });
        }

        [TestMethod]
        public void GetByIds2Test()
        {
            var repo = new UserRepository();
            var data = repo.GetByIds2(new[] { "jholland", "msmith" });
        }

        [TestMethod]
        public void GetByIds3Test()
        {
            var repo = new UserRepository();
            var data = repo.GetByIds3(new[] { "jholland", "msmith" });
        }

        [TestMethod]
        public void GetByIds4Test()
        {
            var repo = new UserRepository();
            var data = repo.GetByIds4(new[] { "jholland", "msmith" });
        }

        [TestMethod]
        public void GetAllWithRolesTest()
        {
            var repo = new UserRepository();
            var data = repo.GetAllWithRoles();
        }

        [TestMethod]
        public void InsertTest()
        {
            var repo = new UserRepository();

            var record = new User
            {
                UserId = "lsmith",
                FirstName = "Luke",
                LastName = "Smith"
            };

            repo.Insert(record);
        }

        [TestMethod]
        public void UpdateTest()
        {
            var repo = new UserRepository();

            var record = new User
            {
                UserId = "lsmith",
                FirstName = "Luke",
                LastName = "Smith Jr"
            };

            repo.Update(record);
        }

        [TestMethod]
        public void Update_CommandBuilderTest()
        {
            var repo = new UserRepository();

            var record = new User
            {
                UserId = "lsmith",
                FirstName = "Luke",
                LastName = "Smith III"
            };

            repo.Update_CommandBuilder_DeriveParameters(record);
        }

        [TestMethod]
        public void DeleteTest()
        {
            var repo = new UserRepository();

            repo.Delete("lsmith");
        }
    }
}
