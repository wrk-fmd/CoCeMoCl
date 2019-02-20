using System.Collections.Generic;
using System.IO;
using System.Linq;
using GeoClient.Models;
using GeoClient.Services;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace GeoClientTests.ServiceTests
{
    [TestFixture]
    public class IncidentItemFactoryTest
    {

        [Test]
        public void CreateIncidentItemList_MultipleIncidents_UnitOnlyShownForAssignedIncidents()
        {
            var units1String = File.ReadAllText(TestContext.CurrentContext.TestDirectory + "..\\TestResources\\units1.json");
            var incidents1String = File.ReadAllText(TestContext.CurrentContext.TestDirectory + "..\\TestResources\\incidents1.json");

            var units = ReadJObjectList(units1String);
            var incidents = ReadJObjectList(incidents1String);

            var incidentItemList = IncidentItemFactory.CreateIncidentItemList(incidents, units);

            var expectedItem1 = CreateExpectedItem1();
            var expectedItem2 = CreateExpectedItem2();
            Assert.Contains(expectedItem1, incidentItemList, "Expected first incident with both units in result.");
            Assert.Contains(expectedItem2, incidentItemList, "Expected second incident only with own unit in results.");
        }

        private static IncidentItem CreateExpectedItem1()
        {
            var expectedItem1 = new IncidentItem("incident-id-1")
            {
                Info = "Info 1",
                Location = new GeoPoint(48, 16),
                Type = GeoIncidentType.Task,
                Units = new SortedSet<Unit>
                {
                    new Unit("unit-id-1")
                    {
                        LastPoint = new GeoPoint(48.5, 16.5),
                        Name = "Own Unit",
                        State = IncidentTaskState.Abo
                    },
                    new Unit("unit-id-2")
                    {
                        Name = "No position unit",
                        State = IncidentTaskState.Assigned
                    }
                }
            };
            return expectedItem1;
        }

        private static IncidentItem CreateExpectedItem2()
        {
            var expectedItem1 = new IncidentItem("incident-id-2")
            {
                Info = "Info 2",
                Location = new GeoPoint(49, 17),
                Type = GeoIncidentType.Task,
                Units = new SortedSet<Unit>
                {
                    new Unit("unit-id-1")
                    {
                        LastPoint = new GeoPoint(48.5, 16.5),
                        Name = "Own Unit",
                        State = IncidentTaskState.Zbo
                    }
                }
            };
            return expectedItem1;
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
