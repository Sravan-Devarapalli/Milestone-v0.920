using System;
using System.Collections.Generic;
using DataAccess;
using DataTransferObjects;
using NUnit.Framework;

namespace DataAccessTest
{
    [TestFixture]
    public class ClientDALTest
    {
		private int testPersonId;

		[TestFixtureSetUp]
		public void Initializer()
		{
			testPersonId = PersonHelper.CreateTestPerson();
		}

		[TestFixtureTearDown]
		public void Finalizer()
		{
			PersonHelper.DeleteTestPerson(testPersonId);
		}

        [Test]
        public void ClientInactivateTest()
        {
            Client client = new Client();
            client.DefaultDiscount = 0.1M;
            client.DefaultTerms = 30;
            client.Name = "bob" + Guid.NewGuid().ToString();
			client.DefaultSalespersonId = testPersonId;

            ClientDAL.ClientInsert(client);
            List<Client> list = ClientDAL.ClientListAll();
            Predicate<Client> clientMatch = delegate(Client c) { return c.Id == client.Id; };
            List<Client> matchList = list.FindAll(clientMatch);
            Assert.AreEqual(1, matchList.Count, "The active client list should contain inserted client");

            ClientDAL.ClientInactivate(client);
            list = ClientDAL.ClientListAll();
            matchList = list.FindAll(clientMatch);
            Assert.AreEqual(0, matchList.Count, "Inactivated client should not appear in active list");
        }

        [Test]
        public void ClientInsertTest()
        {
            Client client = new Client();
            client.DefaultDiscount = 0.1M;
            client.DefaultTerms = 30;
			client.Name = "bob" + Guid.NewGuid().ToString();
			client.DefaultSalespersonId = testPersonId;

            int? initialId = client.Id;
            ClientDAL.ClientInsert(client);
            Assert.AreNotEqual(initialId, client.Id, "system should provide an ID for an insert");
        }

        [Test]
        public void ClientListAllTest()
        {
            Client firstClient = new Client();
            firstClient.DefaultDiscount = 0.1M;
            firstClient.DefaultTerms = 30;
			firstClient.Name = "alice" + Guid.NewGuid().ToString();
			firstClient.DefaultSalespersonId = testPersonId;
            ClientDAL.ClientInsert(firstClient);

            Client secondClient = new Client();
            secondClient.DefaultDiscount = 0.2M;
            secondClient.DefaultTerms = 10;
			secondClient.Name = "bob" + Guid.NewGuid().ToString();
			secondClient.DefaultSalespersonId = testPersonId;
            ClientDAL.ClientInsert(secondClient);

            List<Client> list = ClientDAL.ClientListAll();

            Predicate<Client> firstMatch = delegate(Client c) { return c.Id == firstClient.Id; };
            List<Client> firstList = list.FindAll(firstMatch);
            Assert.AreEqual(1, firstList.Count,
                            "there should be exactly one client in the list matching the first client");

            Predicate<Client> secondMatch = delegate(Client c) { return c.Id == secondClient.Id; };
            List<Client> secondList = list.FindAll(secondMatch);
            Assert.AreEqual(1, secondList.Count,
                            "There should be exactly one client in the list matching the second client");
        }

        [Test]
        public void ClientListAllWithInactiveTest()
        {
            List<Client> originalActiveList = ClientDAL.ClientListAll();
            int originalActiveCount = originalActiveList.Count;

            List<Client> originalInactiveList = ClientDAL.ClientListAllWithInactive();
            int originalInactiveCount = originalInactiveList.Count;

            Client activeClient = new Client();
            activeClient.DefaultDiscount = 0.1M;
            activeClient.DefaultTerms = 30;
			activeClient.Name = "alice" + Guid.NewGuid().ToString();
			activeClient.DefaultSalespersonId = testPersonId;
            ClientDAL.ClientInsert(activeClient);

            Client inactiveClient = new Client();
            inactiveClient.DefaultDiscount = 0.2M;
            inactiveClient.DefaultTerms = 10;
			inactiveClient.Name = "bob" + Guid.NewGuid().ToString();
			inactiveClient.DefaultSalespersonId = testPersonId;
            ClientDAL.ClientInsert(inactiveClient);
            ClientDAL.ClientInactivate(inactiveClient);

            Predicate<Client> activeClientMatch = delegate(Client c) { return c.Id == activeClient.Id; };
            Predicate<Client> inactiveClientMatch = delegate(Client c) { return c.Id == inactiveClient.Id; };

            List<Client> activeList = ClientDAL.ClientListAll();
            Assert.AreEqual(originalActiveCount + 1, activeList.Count, "only new that is active should be counted in active list");
            List<Client> matchList = activeList.FindAll(activeClientMatch);
            Assert.AreEqual(1, matchList.Count, "The activeList client should be found once");
            matchList = activeList.FindAll(inactiveClientMatch);
            Assert.AreEqual(0, matchList.Count, "Inactive client should not be found in the active list");

            List<Client> inactiveList = ClientDAL.ClientListAllWithInactive();
            Assert.AreEqual(originalInactiveCount + 2, inactiveList.Count, "both inactive and active should be counted in inactive list");
            matchList = inactiveList.FindAll(activeClientMatch);
            Assert.AreEqual(1, matchList.Count, "active client should be on inactive list");
            matchList = inactiveList.FindAll(inactiveClientMatch);
            Assert.AreEqual(1,matchList.Count, "inactive client should be on inactive list");
        }

        [Test]
        public void ClientReactivateTest()
        {
            Client client = new Client();
            client.DefaultDiscount = 0.1M;
            client.DefaultTerms = 30;
			client.Name = "bob" + Guid.NewGuid().ToString();
			client.DefaultSalespersonId = testPersonId;

            ClientDAL.ClientInsert(client);
            ClientDAL.ClientInactivate(client);
            ClientDAL.ClientReactivate(client);
            List<Client> list = ClientDAL.ClientListAll();

            Predicate<Client> clientMatch = delegate(Client c) { return c.Id == client.Id; };
            List<Client> matchList = list.FindAll(clientMatch);
            Assert.AreEqual(1, matchList.Count, "Reactivated client should appear in active list");
        }

        [Test]
        public void ClientSanityTest()
        {
            Client client = new Client();
            client.DefaultDiscount = 0.1M;
            client.DefaultTerms = 30;

            Assert.IsFalse(client.Inactive, "clients should start active.");
        }

        [Test]
        public void ClientUpdateTest()
        {
            Client client = new Client();
            client.DefaultDiscount = 0.1M;
            client.DefaultTerms = 30;
			client.Name = "bob" + Guid.NewGuid().ToString();
			client.DefaultSalespersonId = testPersonId;
            ClientDAL.ClientInsert(client);

            List<Client> list = ClientDAL.ClientListAll();
            Predicate<Client> clientMatch = delegate(Client c) { return c.Id == client.Id; };
            List<Client> matchList = list.FindAll(clientMatch);
            Client retrievedClient = matchList[0];

            Assert.AreEqual(client.Id, retrievedClient.Id, "IDs should match");
            Assert.AreEqual(client.DefaultDiscount, retrievedClient.DefaultDiscount, "discounts should match");
            Assert.AreEqual(client.DefaultTerms, retrievedClient.DefaultTerms, "terms should match");
            Assert.AreEqual(client.Inactive, retrievedClient.Inactive, "inactive status should match");
            Assert.AreEqual(client.Name, retrievedClient.Name, "name should match");
            Assert.IsFalse(retrievedClient.Inactive, "retrieved client should be active");

            retrievedClient.DefaultDiscount *= 2;
            retrievedClient.DefaultTerms *= 2;
            retrievedClient.Name += " updated";
			retrievedClient.DefaultSalespersonId = testPersonId;

            ClientDAL.ClientUpdate(retrievedClient);
            List<Client> secondList = ClientDAL.ClientListAll();
            matchList = secondList.FindAll(clientMatch);
            Client updatedClient = matchList[0];

            Assert.AreEqual(retrievedClient.Id, updatedClient.Id, "IDs should match");
            Assert.AreEqual(retrievedClient.DefaultDiscount, updatedClient.DefaultDiscount, "discounts should match");
            Assert.AreEqual(retrievedClient.DefaultTerms, updatedClient.DefaultTerms, "terms should match");
            Assert.AreEqual(retrievedClient.Inactive, updatedClient.Inactive, "inactive status should match");
            Assert.AreEqual(retrievedClient.Name, updatedClient.Name, "name should match");
        }
    }
}

