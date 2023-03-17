﻿namespace Equinor.ProCoSys.PCS5.WebApi.IntegrationTests
{
    public class KnownTestData
    {
        public KnownTestData(string plant) => Plant = plant;

        public string Plant { get; }

        public static string ProjectNameA => "TestProject A";
        public static string ProjectDescriptionA => "Test - Project A";
        public static string FooA => "FOO-A";
        public static string ProjectNameB => "TestProject B";
        public static string ProjectDescriptionB => "Test - Project B";
        public static string FooB => "Foo-B";

        public int FooAId { get; set; }
        public int FooBId { get; set; }
    }
}
