namespace PolpAbp.ZeroAdaptors.Configuration.Dto
{
    public class ThemeSettingsDto
    {
        public string Theme { get; set; }

        public ThemeLayoutSettingsDto Layout { get; set; }

        public ThemeHeaderSettingsDto Header { get; set; }

        public ThemeSubHeaderSettingsDto SubHeader { get; set; } 

        public ThemeMenuSettingsDto Menu { get; set; } 

        public ThemeFooterSettingsDto Footer { get; set; } 

        public ThemeSettingsDto() { 
            Layout= new ThemeLayoutSettingsDto();
            Header= new ThemeHeaderSettingsDto();
            SubHeader= new ThemeSubHeaderSettingsDto();
            Menu= new ThemeMenuSettingsDto();
            Footer= new ThemeFooterSettingsDto();
        }
    }
}