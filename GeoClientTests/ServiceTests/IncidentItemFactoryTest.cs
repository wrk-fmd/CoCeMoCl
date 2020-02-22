using GeoClient.Models;
using GeoClient.Services;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace GeoClientTests.ServiceTests
{
    [TestFixture]
    public class IncidentItemFactoryTest
    {
        [Test]
        public void CreateIncidentItemList_MultipleIncidents_UnitOnlyShownForAssignedIncidents()
        {
            var units1String =
                File.ReadAllText(TestContext.CurrentContext.TestDirectory + "..\\TestResources\\units1.json");
            var incidents1String =
                File.ReadAllText(TestContext.CurrentContext.TestDirectory + "..\\TestResources\\incidents1.json");

            var units = ReadJObjectList(units1String);
            var incidents = ReadJObjectList(incidents1String);

            var incidentItemList = IncidentItemFactory.CreateIncidentItemList(incidents, units, new List<JObject>());

            var expectedItem1 = CreateExpectedItem1();
            var expectedItem2 = CreateExpectedItem2();
            Assert.Contains(expectedItem1, incidentItemList, "Expected first incident with both units in result.");
            Assert.Contains(expectedItem2, incidentItemList, "Expected second incident only with own unit in results.");
        }

        private static IncidentItem CreateExpectedItem1()
        {
            var expectedItem1 = new IncidentItem(
                "incident-id-1",
                GeoIncidentType.Task,
                "Info 1",
                location: new GeoPoint(48, 16),
                destination: new GeoPoint(19, 21),
                units: new List<UnitOfIncident>
                {
                    CreateUnit1(IncidentTaskState.Abo),
                    CreateUnit2()
                });
            return expectedItem1;
        }

        private static IncidentItem CreateExpectedItem2()
        {
            var expectedItem1 = new IncidentItem(
                "incident-id-2",
                info: "Info 2",
                location: new GeoPoint(49, 17),
                type: GeoIncidentType.Task,
                units: new List<UnitOfIncident>
                {
                    CreateUnit1(IncidentTaskState.Zbo)
                });
            return expectedItem1;
        }

        private static UnitOfIncident CreateUnit1(IncidentTaskState expectedState)
        {
            return new UnitOfIncident("unit-id-1",
                "Own Unit",
                new GeoPoint(48.5, 16.5),
                expectedState
            );
        }

        private static UnitOfIncident CreateUnit2()
        {
            return new UnitOfIncident("unit-id-2",
                "No position unit",
                state: IncidentTaskState.Assigned
            );
        }

        private static List<JObject> ReadJObjectList(string inputString)
        {
            var jArray = JArray.Parse(inputString);

            var list = new List<JObject>();
            foreach (var jToken in jArray)
            {
                list.Add((JObject) jToken);
            }

            return list;
        }
    }
}