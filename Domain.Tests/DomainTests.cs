//namespace Domain.Tests
//{
//    public class DomainTests
//    {
//        [Fact]
//        public void StudentHasToBeSixteenOrOlder()
//        {
//            Assert.Throws<ArgumentException>(() =>
//                {
//                    Student student = new Student
//                    {
//                        Id = 1,
//                        FirstName = "Mark",
//                        LastName = "Visser",
//                        Email = "markvisser@gmail.com",
//                        Birthday = DateTime.Now.AddYears(-10),
//                        StudentNumber = 2184736,
//                        StudyCity = City.Breda,
//                        PhoneNumber = "+31648653795"
//                    };
//                });
//        }
//    }
//}