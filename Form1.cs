using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using Microsoft.Win32;

namespace WifiIPChanger;

public partial class Form1 : Form
{

    public Form1()
    {
        InitializeComponent();
        this.Text = "Ağ Ayarları (IP ve MAC Değiştirici)";
        PopulateNetworkInterfaces();

        // Set up the interface selection changed event
        cmbInterfaces.SelectedIndexChanged += cmbInterfaces_SelectedIndexChanged;
        
        // Set up MAC address controls events
        btnShowMAC.Click += btnShowMAC_Click;
        btnGenerateMAC.Click += btnGenerateMAC_Click;
        btnApplyMAC.Click += btnApplyMAC_Click;
        btnReleaseRenew.Click += btnReleaseRenew_Click;
        btnFlushDNS.Click += btnFlushDNS_Click;
    }

    private void PopulateNetworkInterfaces()
    {
        cmbInterfaces.Items.Clear();

        try
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface adapter in interfaces)
            {
                // Add all network interfaces that are up
                if (adapter.OperationalStatus == OperationalStatus.Up)
                {
                    cmbInterfaces.Items.Add(adapter.Name);
                }
            }

            if (cmbInterfaces.Items.Count > 0)
            {
                cmbInterfaces.SelectedIndex = 0;
                lblStatus.Text = "Hazır";

                // Update IP display for the initially selected interface
                if (cmbInterfaces.SelectedItem != null)
                {
                    UpdateCurrentIPDisplay(cmbInterfaces.SelectedItem.ToString());
                    UpdateSSIDDisplay(cmbInterfaces.SelectedItem.ToString());
                }
            }
            else
            {
                lblStatus.Text = "Aktif ağ arayüzü bulunamadı";
            }
        }
        catch (Exception ex)
        {
            lblStatus.Text = $"Hata: {ex.Message}";
        }
    }

    private void rbDHCP_CheckedChanged(object sender, EventArgs e)
    {
        bool enableStaticFields = !rbDHCP.Checked;

        txtIPAddress.Enabled = enableStaticFields;
        txtSubnetMask.Enabled = enableStaticFields;
        txtGateway.Enabled = enableStaticFields;

        lblIPAddress.Enabled = enableStaticFields;
        lblSubnetMask.Enabled = enableStaticFields;
        lblGateway.Enabled = enableStaticFields;
        btnRandomIP.Enabled = enableStaticFields;
    }

    private void btnRandomIP_Click(object sender, EventArgs e)
    {
        Random random = new Random();
        int lastOctet = random.Next(1, 255); // Generate a number between 1 and 254
        txtIPAddress.Text = $"192.168.8.{lastOctet}";
        txtSubnetMask.Text = "255.255.255.0";
        txtGateway.Text = "192.168.8.1";

    }

    private void btnApply_Click(object sender, EventArgs e)
    {
        if (cmbInterfaces.SelectedItem == null)
        {
            lblStatus.Text = "Hata: Arayüz seçilmedi";
            return;
        }

        string interfaceName = cmbInterfaces.SelectedItem.ToString();
        bool useDhcp = rbDHCP.Checked;

        try
        {
            lblStatus.Text = "Değişiklikler uygulanıyor...";
            Application.DoEvents();

            if (useDhcp)
            {
                SetDHCP(interfaceName);
            }
            else
            {
                // Validate IP inputs
                if (string.IsNullOrWhiteSpace(txtIPAddress.Text) ||
                    string.IsNullOrWhiteSpace(txtSubnetMask.Text) ||
                    string.IsNullOrWhiteSpace(txtGateway.Text))
                {
                    lblStatus.Text = "Hata: Statik IP için tüm alanlar gereklidir";
                    return;
                }

                SetStaticIP(interfaceName, txtIPAddress.Text, txtSubnetMask.Text, txtGateway.Text);
            }

            // Restart the adapter
            RestartAdapter(interfaceName);

            lblStatus.Text = "IP yapılandırması başarıyla uygulandı";

            // Update the current IP display after applying changes
            if (cmbInterfaces.SelectedItem != null)
            {
                UpdateCurrentIPDisplay(cmbInterfaces.SelectedItem.ToString());
                UpdateSSIDDisplay(cmbInterfaces.SelectedItem.ToString());
            }
        }
        catch (Exception ex)
        {
            lblStatus.Text = $"Hata: {ex.Message}";
        }
    }

    private void SetDHCP(string interfaceName)
    {
        RunNetshCommand($"interface ipv4 set address name=\"{interfaceName}\" source=dhcp");
        RunNetshCommand($"interface ipv4 set dnsservers name=\"{interfaceName}\" source=dhcp");
    }

    private void SetStaticIP(string interfaceName, string ipAddress, string subnetMask, string gateway)
    {
        RunNetshCommand($"interface ipv4 set address name=\"{interfaceName}\" static {ipAddress} {subnetMask} {gateway}");
    }

    private void RestartAdapter(string interfaceName)
    {
        RunNetshCommand($"interface set interface name=\"{interfaceName}\" admin=disabled");
        RunNetshCommand($"interface set interface name=\"{interfaceName}\" admin=enabled");
    }

    private void RunNetshCommand(string arguments)
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "netsh",
            Arguments = arguments,
            Verb = "runas", // Run as admin
            CreateNoWindow = true,
            UseShellExecute = true
        };

        using (Process process = Process.Start(psi))
        {
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception($"Komut {process.ExitCode} çıkış koduyla başarısız oldu");
            }
        }
    }

    private void btnRefresh_Click(object sender, EventArgs e)
    {
        PopulateNetworkInterfaces();
    }

    private void cmbInterfaces_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cmbInterfaces.SelectedItem != null)
        {
            string selectedInterface = cmbInterfaces.SelectedItem.ToString();
            UpdateCurrentIPDisplay(selectedInterface);
            UpdateSSIDDisplay(selectedInterface);
        }
    }

    private void UpdateCurrentIPDisplay(string interfaceName)
    {
        try
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in interfaces)
            {
                if (adapter.Name == interfaceName)
                {
                    IPAddress ipAddress = null;
                    foreach (UnicastIPAddressInformation ipInfo in adapter.GetIPProperties().UnicastAddresses)
                    {
                        if (ipInfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            ipAddress = ipInfo.Address;
                            break;
                        }
                    }

                    if (ipAddress != null)
                    {
                        lblCurrentIP.Text = $"Mevcut IP: {ipAddress}";
                    }
                    else
                    {
                        lblCurrentIP.Text = "Mevcut IP: Mevcut Değil";
                    }

                    return;
                }
            }

            lblCurrentIP.Text = "Mevcut IP: Mevcut Değil";
        }
        catch (Exception ex)
        {
            lblCurrentIP.Text = "Mevcut IP: Alınırken hata oluştu";
            Debug.WriteLine($"IP alınırken hata: {ex.Message}");
        }
    }

    private void UpdateSSIDDisplay(string interfaceName)
    {
        Debug.WriteLine($"Starting UpdateSSIDDisplay for interface: {interfaceName}");
        
        try
        {
            // Configure process to execute netsh with appropriate settings
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "netsh",
                Arguments = "wlan show interfaces",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            Debug.WriteLine("Executing netsh wlan show interfaces command...");
            string output = "";
            string errorOutput = "";
            int exitCode = -1;
            
            try
            {
                using (Process process = Process.Start(psi))
                {
                    if (process != null)
                    {
                        output = process.StandardOutput.ReadToEnd();
                        errorOutput = process.StandardError.ReadToEnd();
                        process.WaitForExit();
                        exitCode = process.ExitCode;
                        
                        Debug.WriteLine($"Command executed with exit code: {exitCode}");
                        Debug.WriteLine($"Output length: {output.Length} characters");
                        if (!string.IsNullOrEmpty(errorOutput))
                            Debug.WriteLine($"Error output: {errorOutput}");
                    }
                    else
                    {
                        Debug.WriteLine("Process.Start returned null");
                        lblSSID.Text = "SSID: [İşlem başlatılamadı]";
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception executing netsh command: {ex.Message}");
                lblSSID.Text = "SSID: [Yönetici izinleri gerekiyor]";
                
                ToolTip toolTip = new ToolTip();
                toolTip.SetToolTip(lblSSID, "Bu özellik yönetici izinleri gerektirir. Uygulamayı yönetici olarak çalıştırın.");
                return;
            }

            // Check for command execution failure and analyze error output
            if (exitCode != 0 || !string.IsNullOrEmpty(errorOutput))
            {
                Debug.WriteLine($"netsh command failed with exit code: {exitCode}");
                
                // Check if error is related to location services
                if (errorOutput.Contains("location") || errorOutput.Contains("konum") || 
                    output.Contains("location") || output.Contains("konum"))
                {
                    Debug.WriteLine("Detected location services error");
                    lblSSID.Text = "SSID: [Konum hizmetleri gerekiyor]";
                    
                    ToolTip toolTip = new ToolTip();
                    toolTip.SetToolTip(lblSSID, "Windows Konum Hizmetleri'ni etkinleştirmeniz gerekiyor. Ayarlar > Gizlilik > Konum");
                }
                // Check if error is related to admin privileges
                else if (errorOutput.Contains("admin") || errorOutput.Contains("yönetici") || 
                        errorOutput.Contains("elevat") || errorOutput.Contains("privilege"))
                {
                    Debug.WriteLine("Detected admin privileges error");
                    lblSSID.Text = "SSID: [Yönetici izinleri gerekiyor]";
                    
                    ToolTip toolTip = new ToolTip();
                    toolTip.SetToolTip(lblSSID, "Bu özellik yönetici izinleri gerektirir. Uygulamayı yönetici olarak çalıştırın.");
                }
                // Generic error handling
                else
                {
                    Debug.WriteLine($"Unknown error: {errorOutput}");
                    lblSSID.Text = "SSID: [Bilgi alınamadı]";
                    
                    ToolTip toolTip = new ToolTip();
                    toolTip.SetToolTip(lblSSID, $"SSID bilgisi alınamadı. Hata: {errorOutput}");
                }
                return;
            }

            // Check for empty or invalid output
            if (string.IsNullOrWhiteSpace(output))
            {
                Debug.WriteLine("netsh command returned empty output");
                lblSSID.Text = "SSID: [Bilgi alınamadı]";
                return;
            }

            // Check for disconnected state directly in the output
            if (output.Contains("State : disconnected") || output.Contains("State : Disconnected"))
            {
                Debug.WriteLine("Detected disconnected WiFi state");
                lblSSID.Text = "SSID: [Bağlı değil]";
                
                ToolTip toolTip = new ToolTip();
                toolTip.SetToolTip(lblSSID, "WiFi herhangi bir ağa bağlı değil. Lütfen bir ağa bağlanın.");
                return;
            }

            Debug.WriteLine("Processing netsh command output...");
            Debug.WriteLine($"Raw output sample: {output.Substring(0, Math.Min(output.Length, 200))}");

            // Parse the output using multiple approaches
            ParseResult result = ParseNetshOutput(output, interfaceName);
            
            if (result.Success && !string.IsNullOrEmpty(result.SSID))
            {
                Debug.WriteLine($"Successfully found SSID: {result.SSID}");
                lblSSID.Text = $"SSID: {result.SSID}";
            }
            else
            {
                Debug.WriteLine($"Failed to find SSID. Status: {result.StatusMessage}");
                lblSSID.Text = result.StatusMessage.StartsWith("SSID:") 
                            ? result.StatusMessage 
                            : $"SSID: {result.StatusMessage}";
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unhandled exception in UpdateSSIDDisplay: {ex}");
            lblSSID.Text = "SSID: Alınırken hata oluştu";
        }
    }

    // Helper class to store parsing results
    private class ParseResult
    {
        public bool Success { get; set; }
        public string SSID { get; set; }
        public string StatusMessage { get; set; }
        
        public ParseResult(bool success, string ssid, string statusMessage)
        {
            Success = success;
            SSID = ssid;
            StatusMessage = statusMessage;
        }
    }

    // Improved parsing logic with multiple methods
    private ParseResult ParseNetshOutput(string output, string targetInterfaceName)
    {
        Debug.WriteLine($"Parsing netsh output for interface: {targetInterfaceName}");
        
        // Check if output contains any SSID information
        if (!output.Contains("SSID"))
        {
            Debug.WriteLine("Output does not contain any SSID information");
            return new ParseResult(false, "", "SSID: [Konum hizmetleri gerekiyor]");
        }
        
        // Attempt standard parsing first
        ParseResult result = ParseStandardFormat(output, targetInterfaceName);
        if (result.Success)
        {
            return result;
        }
        
        // Try alternative parsing methods if standard failed
        result = ParseAlternativeFormat(output, targetInterfaceName);
        if (result.Success)
        {
            return result;
        }
        
        // Last resort parsing (just extract any SSID if we can find one)
        result = ParseFallbackMethod(output);
        if (result.Success)
        {
            return result;
        }
        
        // If we get here, no parsing method worked
        return new ParseResult(false, "", "[Bağlı değil]");
    }

    // Standard parsing method - looks for Interface Name then SSID
    private ParseResult ParseStandardFormat(string output, string targetInterfaceName)
    {
        Debug.WriteLine("Attempting standard format parsing");
        string[] lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        
        string currentInterface = "";
        string ssid = "";
        
        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();
            Debug.WriteLine($"Examining line: {trimmedLine}");
            
            // Look for interface name
            if (trimmedLine.StartsWith("Name"))
            {
                int colonIndex = trimmedLine.IndexOf(':');
                if (colonIndex != -1)
                {
                    currentInterface = trimmedLine.Substring(colonIndex + 1).Trim();
                    Debug.WriteLine($"Found interface: {currentInterface}");
                }
            }
            // Look for SSID (but not BSSID)
            else if (trimmedLine.StartsWith("SSID") && !trimmedLine.Contains("BSSID"))
            {
                int colonIndex = trimmedLine.IndexOf(':');
                if (colonIndex != -1)
                {
                    ssid = trimmedLine.Substring(colonIndex + 1).Trim();
                    Debug.WriteLine($"Found SSID: {ssid} for interface: {currentInterface}");
                    
                    // If this is the interface we're looking for, we're done
                    if (string.Equals(currentInterface, targetInterfaceName, StringComparison.OrdinalIgnoreCase))
                    {
                        return new ParseResult(true, ssid, "");
                    }
                }
            }
            // Check for connected state - if we see "State : Disconnected" for our interface
            else if (trimmedLine.StartsWith("State") && 
                    string.Equals(currentInterface, targetInterfaceName, StringComparison.OrdinalIgnoreCase))
            {
                int colonIndex = trimmedLine.IndexOf(':');
                if (colonIndex != -1)
                {
                    string state = trimmedLine.Substring(colonIndex + 1).Trim();
                    Debug.WriteLine($"Found state: {state} for interface: {currentInterface}");
                    
                    if (state.Contains("Disconnected") || state.Contains("disconnected"))
                    {
                        return new ParseResult(false, "", "[Bağlı değil]");
                    }
                }
            }
        }
        
        // If we didn't return earlier but found an SSID for a different interface
        if (!string.IsNullOrEmpty(ssid))
        {
            Debug.WriteLine("Found SSID but for a different interface");
            return new ParseResult(false, "", "Arayüz bulunamadı");
        }
        
        Debug.WriteLine("Standard parsing didn't find SSID");
        return new ParseResult(false, "", "");
    }

    // Alternative parsing for different netsh output formats
    private ParseResult ParseAlternativeFormat(string output, string targetInterfaceName)
    {
        Debug.WriteLine("Attempting alternative format parsing");
        string[] lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        
        bool foundInterfaceSection = false;
        string currentInterface = "";
        string ssid = "";
        
        // This approach looks for patterns like "Name : <interface>" followed by "SSID : <ssid>"
        // without requiring specific ordering or exact field names
        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();
            Debug.WriteLine($"Alt parsing examining line: {trimmedLine}");
            
            // Check for any line that contains "Name" and ":" to find interface names
            if (trimmedLine.Contains("Name") && trimmedLine.Contains(":"))
            {
                int colonIndex = trimmedLine.IndexOf(':');
                if (colonIndex != -1)
                {
                    currentInterface = trimmedLine.Substring(colonIndex + 1).Trim();
                    Debug.WriteLine($"Alt found potential interface: {currentInterface}");
                    foundInterfaceSection = true;
                }
            }
            // After we've found an interface section, look for any SSID information
            else if (foundInterfaceSection && 
                    (trimmedLine.Contains("SSID") || trimmedLine.Contains("Profile")) && 
                    trimmedLine.Contains(":") && 
                    !trimmedLine.Contains("BSSID"))
            {
                int colonIndex = trimmedLine.IndexOf(':');
                if (colonIndex != -1)
                {
                    ssid = trimmedLine.Substring(colonIndex + 1).Trim();
                    Debug.WriteLine($"Alt found potential SSID: {ssid} for interface: {currentInterface}");
                    
                    // If this might be the interface we're interested in, return it
                    if (string.IsNullOrEmpty(targetInterfaceName) || 
                        currentInterface.Contains(targetInterfaceName) ||
                        targetInterfaceName.Contains(currentInterface))
                    {
                        return new ParseResult(true, ssid, "");
                    }
                }
            }
            
            // Skip to next interface section if we see indication of a different state
            if (foundInterfaceSection && 
                (trimmedLine.Contains("-----") || trimmedLine.Contains("====") || trimmedLine.Length == 0))
            {
                foundInterfaceSection = false;
            }
        }
        
        // If we found any SSID but not for the target interface
        if (!string.IsNullOrEmpty(ssid))
        {
            return new ParseResult(false, "", "Arayüz eşleşmedi");
        }
        
        return new ParseResult(false, "", "");
    }

    // Last resort parsing that just looks for any SSID in the output
    private ParseResult ParseFallbackMethod(string output)
    {
        Debug.WriteLine("Attempting fallback parsing method");
        string[] lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        
        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();
            
            // Look for any line with "SSID" and ":" that doesn't contain "BSSID"
            if (trimmedLine.Contains("SSID") && 
                !trimmedLine.Contains("BSSID") && 
                trimmedLine.Contains(":"))
            {
                int colonIndex = trimmedLine.IndexOf(':');
                if (colonIndex != -1 && colonIndex + 1 < trimmedLine.Length)
                {
                    string ssid = trimmedLine.Substring(colonIndex + 1).Trim();
                    Debug.WriteLine($"Fallback found SSID: {ssid}");
                    
                    if (!string.IsNullOrEmpty(ssid))
                    {
                        return new ParseResult(true, ssid, "");
                    }
                }
            }
        }
        
        // If we couldn't find any SSID
        return new ParseResult(false, "", "SSID bulunamadı");
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        // MAC Address changing controls initialization can go here if needed
        string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        
        Text = Text + String.Format(" - sürüm {0}", version);

    }

    private void btnShowMAC_Click(object sender, EventArgs e)
    {
        if (cmbInterfaces.SelectedItem == null)
        {
            lblStatus.Text = "Hata: Arayüz seçilmedi";
            return;
        }

        string interfaceName = cmbInterfaces.SelectedItem.ToString();
        string macAddress = GetMacAddress(interfaceName);
        
        if (!string.IsNullOrEmpty(macAddress))
        {
            txtMacAddress.Text = FormatMacAddress(macAddress);
            lblStatus.Text = "MAC adresi başarıyla alındı";
        }
        else
        {
            lblStatus.Text = "MAC adresi alınamadı. Yönetici izinlerinizi kontrol edin.";
        }
    }

    private string GetMacAddress(string interfaceName)
    {
        try
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in interfaces)
            {
                if (adapter.Name == interfaceName)
                {
                    return adapter.GetPhysicalAddress().ToString();
                }
            }
            return string.Empty;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"MAC adresi alınırken hata: {ex.Message}");
            lblStatus.Text = $"MAC adresi alınırken hata: {ex.Message}";
            return string.Empty;
        }
    }

    private string GetMacAddressFromRegistry(string interfaceName)
    {
        try
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in interfaces)
            {
                if (adapter.Name == interfaceName)
                {
                    string adapterId = adapter.Id;
                    string registryKey = $@"SYSTEM\CurrentControlSet\Control\Class\{{4D36E972-E325-11CE-BFC1-08002BE10318}}";
                    
                    using (RegistryKey baseKey = Registry.LocalMachine.OpenSubKey(registryKey))
                    {
                        if (baseKey != null)
                        {
                            string[] subkeyNames = baseKey.GetSubKeyNames();
                            
                            foreach (string subkeyName in subkeyNames)
                            {
                                if (subkeyName == "Properties" || subkeyName == "Configuration")
                                    continue;
                                    
                                using (RegistryKey subkey = baseKey.OpenSubKey(subkeyName))
                                {
                                    if (subkey == null) continue;
                                    
                                    string instanceId = (string)subkey.GetValue("NetCfgInstanceId", "");
                                    
                                    if (string.Equals(instanceId, adapterId, StringComparison.OrdinalIgnoreCase))
                                    {
                                        string networkAddress = (string)subkey.GetValue("NetworkAddress", "");
                                        if (!string.IsNullOrEmpty(networkAddress))
                                        {
                                            return networkAddress;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Registry'den MAC adresi alınırken hata: {ex.Message}");
            lblStatus.Text = $"Registry'den MAC adresi alınırken hata: {ex.Message}";
            return string.Empty;
        }
    }

    private void SetMacAddress(string interfaceName, string macAddress)
    {
        try
        {
            // Remove any formatting from MAC address
            macAddress = macAddress.Replace(":", "").Replace("-", "").Replace(".", "").ToUpper();
            
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in interfaces)
            {
                if (adapter.Name == interfaceName)
                {
                    string adapterId = adapter.Id;
                    string registryKey = $@"SYSTEM\CurrentControlSet\Control\Class\{{4D36E972-E325-11CE-BFC1-08002BE10318}}";
                    
                    using (RegistryKey baseKey = Registry.LocalMachine.OpenSubKey(registryKey, true))
                    {
                        if (baseKey != null)
                        {
                            string[] subkeyNames = baseKey.GetSubKeyNames();
                            
                            foreach (string subkeyName in subkeyNames)
                            {
                                if (subkeyName == "Properties" || subkeyName == "Configuration")
                                    continue;
                                    
                                using (RegistryKey subkey = baseKey.OpenSubKey(subkeyName, true))
                                {
                                    if (subkey == null) continue;
                                    
                                    string instanceId = (string)subkey.GetValue("NetCfgInstanceId", "");
                                    
                                    if (string.Equals(instanceId, adapterId, StringComparison.OrdinalIgnoreCase))
                                    {
                                        // Store the MAC address in the registry
                                        subkey.SetValue("NetworkAddress", macAddress, RegistryValueKind.String);
                                        
                                        // Disable and enable the adapter to apply changes
                                        RestartAdapter(interfaceName);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            throw new Exception("Ağ arayüzü registry'de bulunamadı. Yönetici olarak çalıştırdığınızdan emin olun.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"MAC adresi değiştirilirken hata: {ex.Message}");
            throw new Exception($"MAC adresi değiştirilirken hata oluştu: {ex.Message}\nYönetici haklarıyla çalıştırdığınızdan emin olun.");
        }
    }

    private void btnGenerateMAC_Click(object sender, EventArgs e)
    {
        Random random = new Random();
        byte[] macBytes = new byte[6];
        random.NextBytes(macBytes);
        
        // Ensure first byte adheres to standards (locally administered, unicast address)
        macBytes[0] = (byte)(macBytes[0] & 0xFE | 0x02);
        
        string macAddress = BitConverter.ToString(macBytes).Replace("-", ":");
        txtMacAddress.Text = macAddress;
    }

    private void btnApplyMAC_Click(object sender, EventArgs e)
    {
        if (cmbInterfaces.SelectedItem == null)
        {
            lblStatus.Text = "Hata: Arayüz seçilmedi";
            return;
        }

        string interfaceName = cmbInterfaces.SelectedItem.ToString();
        string macAddress = txtMacAddress.Text.Trim();
        
        if (string.IsNullOrWhiteSpace(macAddress))
        {
            lblStatus.Text = "Hata: MAC adresi boş olamaz";
            return;
        }
        
        if (!IsValidMacAddress(macAddress))
        {
            lblStatus.Text = "Hata: Geçersiz MAC adresi formatı. XX:XX:XX:XX:XX:XX formatında girdiğinizden emin olun.";
            return;
        }
        
        try
        {
            lblStatus.Text = "MAC adresi değiştiriliyor...";
            Application.DoEvents();
            
            SetMacAddress(interfaceName, macAddress);
            
            lblStatus.Text = "MAC adresi başarıyla değiştirildi. Arayüz yeniden başlatılıyor...";
        }
        catch (Exception ex)
        {
            lblStatus.Text = $"Hata: {ex.Message}";
            MessageBox.Show($"MAC adresi değiştirilirken bir hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private bool IsValidMacAddress(string macAddress)
    {
        // Remove common MAC address separators
        string cleanMac = macAddress.Replace(":", "").Replace("-", "").Replace(".", "");
        
        // Check if the resulting string is 12 hexadecimal characters
        return System.Text.RegularExpressions.Regex.IsMatch(cleanMac, "^[0-9A-Fa-f]{12}$");
    }

    private string FormatMacAddress(string macAddress)
    {
        // Format MAC address with colons (XX:XX:XX:XX:XX:XX)
        if (string.IsNullOrEmpty(macAddress)) return string.Empty;
        
        macAddress = macAddress.Replace(":", "").Replace("-", "").Replace(".", "");
        
        string formattedMac = "";
        for (int i = 0; i < macAddress.Length; i += 2)
        {
            if (i + 2 <= macAddress.Length)
            {
                formattedMac += macAddress.Substring(i, 2);
                if (i + 2 < macAddress.Length) formattedMac += ":";
            }
        }
        
        return formattedMac;
    }
private void btnReleaseRenew_Click(object sender, EventArgs e)
{
    if (cmbInterfaces.SelectedItem == null)
    {
        lblStatus.Text = "Hata: Arayüz seçilmedi";
        return;
    }

    string interfaceName = cmbInterfaces.SelectedItem.ToString();
    
    try
    {
        lblStatus.Text = "IP adresi yenileniyor...";
        Application.DoEvents();
        
        // Release current IP address
        ProcessStartInfo psiRelease = new ProcessStartInfo
        {
            FileName = "ipconfig",
            Arguments = $"/release \"{interfaceName}\"",
            Verb = "runas", // Run as admin
            CreateNoWindow = true,
            UseShellExecute = true
        };
        
        using (Process processRelease = Process.Start(psiRelease))
        {
            processRelease.WaitForExit();
        }
        
        // Small delay to ensure release completes
        System.Threading.Thread.Sleep(1000);
        
        // Renew IP address
        ProcessStartInfo psiRenew = new ProcessStartInfo
        {
            FileName = "ipconfig",
            Arguments = $"/renew \"{interfaceName}\"",
            Verb = "runas", // Run as admin
            CreateNoWindow = true,
            UseShellExecute = true
        };
        
        using (Process processRenew = Process.Start(psiRenew))
        {
            processRenew.WaitForExit();
        }
        
        lblStatus.Text = "IP adresi başarıyla yenilendi";
        
        // Update the current IP display after applying changes
        if (cmbInterfaces.SelectedItem != null)
        {
            UpdateCurrentIPDisplay(cmbInterfaces.SelectedItem.ToString());
            UpdateSSIDDisplay(cmbInterfaces.SelectedItem.ToString());
        }
    }
    catch (Exception ex)
    {
        lblStatus.Text = $"Hata: {ex.Message}";
        MessageBox.Show($"IP adresi yenilenirken bir hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
private void btnFlushDNS_Click(object sender, EventArgs e)
{
    try
    {
        lblStatus.Text = "DNS önbelleği temizleniyor...";
        Application.DoEvents();
        
        // Flush DNS cache
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "ipconfig",
            Arguments = "/flushdns",
            Verb = "runas", // Run as admin
            CreateNoWindow = true,
            UseShellExecute = true
        };
        
        using (Process process = Process.Start(psi))
        {
            process.WaitForExit();
        }
        
        lblStatus.Text = "DNS önbelleği başarıyla temizlendi";
    }
    catch (Exception ex)
    {
        lblStatus.Text = $"Hata: {ex.Message}";
        MessageBox.Show($"DNS önbelleği temizlenirken bir hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
}
