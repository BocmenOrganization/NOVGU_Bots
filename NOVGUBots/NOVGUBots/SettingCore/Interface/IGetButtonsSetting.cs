﻿namespace NOVGUBots.SettingCore.Interface
{
    public interface IGetButtonsSetting
    {
        public (string AppName, string PageName)[] GetPages();
    }
}
