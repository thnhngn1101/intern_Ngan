using BPMaster.Common.Application.Settings;

namespace Common.Application.Settings
{
    public abstract class BaseAppSetting
    {
        public abstract BasePermissionSetting PermissionSetting { get; }
        public JwtTokenSetting JwtTokenSetting { get; set; } = new();

        public string? FolderGenerateSqlScript { get; set; }
        public ExternalServicesSetting? ExternalServicesSetting { get; set; }
        public DatabaseSetting? DatabaseSetting { get; set; } = new();
        public DatabaseWarehouseSetting? DatabaseWarehouseSetting { get; set; } = new();
        public SAPInterfaceSetting? SAPInterfaceSetting { get; set; } = new();
        public SAPInterfaceConnection? SAPInterfaceConnection { get; set; } = new();
        public SystemIntergrationSetting? SystemIntergrationSetting { get; set; } = new();
        public LoginInfoSetting? LoginInfoSetting { get; set; } = new();
        public EmailServerSetting? EmailServerSetting { get; set; } = new();
        public string STORAGE_ROOT { get; set; } = string.Empty;
        public string STORAGE_PUBLIC { get; set; } = string.Empty;
        public string STORAGE_ATTACHMENTS { get; set; } = string.Empty;
        public OMSInterfaceSetting OMSInterfaceSetting { get; set; } = new();
        public string? DOMAIN_URL { get; set; } = string.Empty;
    }
}
