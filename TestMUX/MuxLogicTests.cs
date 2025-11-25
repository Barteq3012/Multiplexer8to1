using Multiplexer8to1;

namespace TestMUX
{
    [TestClass]
    public class MuxLogicTests
    {
        [TestMethod]
        public void Result_ShouldBe_1_When_D0_Is_True_And_Selector_Is_0()
        {
            //given
            bool d0 = true;
            bool s0 = false;
            bool s1 = false;
            bool s2 = false;

            //when
            byte result = MuxLogic.SolveFromFrontend(
                d0, false, false, false, false, false, false, false,
                s0, s1, s2
            );

            //then
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void Result_ShouldBe_1_When_D7_Is_True_And_Selector_Is_7()
        {
            bool d7 = true;
            bool s0 = true;
            bool s1 = true;
            bool s2 = true;

            byte result = MuxLogic.SolveFromFrontend(
                false, false, false, false, false, false, false, d7,
                s0, s1, s2
            );

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void Result_ShouldBe_0_When_D3_Is_False_And_Selector_Is_3()
        {
            bool d3 = false;
            bool s0 = true;
            bool s1 = true;
            bool s2 = false;

            byte result = MuxLogic.SolveFromFrontend(
                true, true, true, d3, true, true, true, true,
                s0, s1, s2
            );

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Result_ShouldBe_1_When_D5_Is_True_And_Selector_Is_5()
        {
            bool d5 = true;
            bool s0 = true;
            bool s1 = false;
            bool s2 = true;

            byte result = MuxLogic.SolveFromFrontend(
                false, false, false, false, false, d5, false, false,
                s0, s1, s2
            );

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void Result_ShouldBe_1_When_D6_Is_True_And_Selector_Is_6()
        {
            bool d6 = true;
            bool s0 = false;
            bool s1 = true;
            bool s2 = true;

            byte result = MuxLogic.SolveFromFrontend(
                false, false, false, false, false, false, d6, false,
                s0, s1, s2
            );

            Assert.AreEqual(1, result);
        }
    }
}