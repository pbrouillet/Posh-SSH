﻿using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace SSH.Stores
{
    public class RegistryStore : Stores.IStore
    {
        public IDictionary<string, string> GetKeys()
        {
            var hostkeys = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            var poshSoftKey = Registry.CurrentUser.OpenSubKey(@"Software\PoshSSH", true);
            if (poshSoftKey != null)
            {
                var hosts = poshSoftKey.GetValueNames();
                foreach (var host in hosts)
                {
                    var hostkey = poshSoftKey.GetValue(host).ToString();
                    hostkeys.Add(host, hostkey);
                }
            }
            else
            {
                using (var softKey = Registry.CurrentUser.OpenSubKey(@"Software", true))
                {
                    if (softKey != null) softKey.CreateSubKey("PoshSSH");
                }
            }
            return hostkeys;
        }

        public bool SetKey(string host, string fingerprint)
        {
            var poshSoftKey = Registry.CurrentUser.OpenSubKey(@"Software\PoshSSH", true);
            if (poshSoftKey != null)
            {
                poshSoftKey.SetValue(host, fingerprint);
                return true;
            }
            var softKey = Registry.CurrentUser.OpenSubKey(@"Software", true);
            if (softKey == null) return true;
            softKey.CreateSubKey("PoshSSH");
            softKey.SetValue(host, fingerprint);
            return true;
        }
    }
}
