namespace TravelManagement.Helper
{
    public class Claculations
    {
        public static double CalculateAge(DateOnly registrationDate, DateTime currentDate)
        {
            int years = currentDate.Year - registrationDate.Year;
            int months = currentDate.Month - registrationDate.Month;
            int days = currentDate.Day - registrationDate.Day;

            if (days < 0)
            {
                months--;
                days += DateTime.DaysInMonth(currentDate.Year, currentDate.Month - 1);
            }

            if (months < 0)
            {
                years--;
                months += 12;
            }

            double ageInYears = years + (months / 12.0) + (days / 365.0);
            return ageInYears;
        }
    }
}
