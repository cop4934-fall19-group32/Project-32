using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestOutputGenerators
    {
        T CreateGenerator<T>() where T : SolutionGenerator {
            GameObject go = new GameObject();
            return go.AddComponent<T>();
        }

        // A Test behaves as an ordinary method
        [Test]
        public void TestAlternatorGenerator() {
            var generator = CreateGenerator<AlternatorGenerator>();

            int[] sampleInput = { 1, 2, 3, 4 };
            List<int> expectedOutput = new List<int> { 2, 4 };
            var output = generator.GenerateSolution(sampleInput);

            Assert.IsTrue(expectedOutput.SequenceEqual(output));
        }

        // A Test behaves as an ordinary method
        [Test]
        public void TestFlipFlopHopGenerator() {
            var generator = CreateGenerator<FlipFlopHopGenerator>();

            int[] sampleInput = { 1, 2, 3, 4 };
            List<int> expectedOutput = new List<int> { 2, 1, 4, 3 };
            var output = generator.GenerateSolution(sampleInput);

            Assert.IsTrue(expectedOutput.SequenceEqual(output));
        }

        // A Test behaves as an ordinary method
        [Test]
        public void TestFlippedInputGenerator()
        {
            var generator = CreateGenerator<FlippedInputGenerator>();
            
            int[] sampleInput = { 1, 2 };
            List<int> expectedOutput = new List<int>{ 2, 1 };
            var output = generator.GenerateSolution(sampleInput);

            Assert.IsTrue(expectedOutput.SequenceEqual(output));
        }
    
    }
}
