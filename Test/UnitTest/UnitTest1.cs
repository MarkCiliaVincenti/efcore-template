namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            // ��ȡ��ǰ��Unixʱ���
            long unixTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // ��Unixʱ���ת��ΪDateTimeOffset����
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(unixTimeMilliseconds);

            // ��ʽ��Ϊ yyyy-MM-dd HH:mm:ss �ַ���
            string formattedDateTime = dateTimeOffset.ToString("yyyy-MM-dd HH:mm:ss");


        }
    }
}