namespace vstest.diag.Utilities
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Xml;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class XmlTrace
    {
        private const string BackupSuffix = "_bk";

        public static bool EnableXmlTrace(string configFilePath)
        {
            // File existence check
            var flag = false;
            if (!File.Exists(configFilePath))
            {
                return flag;
            }

            // Valid XML file check
            var configXml = new XmlDocument();
            try
            {
                configXml.Load(configFilePath);


                // Check if a valid config file of expected type with configuration node
                if (configXml.GetElementsByTagName("configuration").Count < 1)
                {
                    return flag;
                }

                // Check if configuration file can be backed up
                File.Copy(configFilePath, configFilePath + BackupSuffix, true);

                // 
                var configXmlNode = configXml.GetElementsByTagName("configuration")[0];
                var sysDiagnosticsXmlNode = configXmlNode[@"system.diagnostics"];

                // Check if the system.diagnostics sub-node exists
                if (sysDiagnosticsXmlNode != null && sysDiagnosticsXmlNode.OuterXml != string.Empty)
                {
                    var switchesXmlNode = sysDiagnosticsXmlNode["switches"];
                    // Check if the switches sub-node exists
                    if (switchesXmlNode != null && switchesXmlNode.OuterXml != string.Empty)
                    {
                        var addTraceNode = switchesXmlNode.SelectSingleNode("descendant::add[@name='TpTraceLevel']");
                        var addTraceElemNew = configXml.CreateElement("add");
                        var traceAttr = configXml.CreateAttribute("name");
                        traceAttr.Value = "TpTraceLevel";
                        var valAttr = configXml.CreateAttribute("value");
                        valAttr.Value = "4";
                        addTraceElemNew.Attributes.Append(traceAttr);
                        addTraceElemNew.Attributes.Append(valAttr);
                        if (addTraceNode != null && addTraceNode.OuterXml != string.Empty)
                        {
                            switchesXmlNode.ReplaceChild(addTraceElemNew, addTraceNode);
                        }
                        else
                        {
                            switchesXmlNode.AppendChild(addTraceElemNew);
                        }

                        sysDiagnosticsXmlNode.ReplaceChild(switchesXmlNode, sysDiagnosticsXmlNode["switches"]);
                        configXml["configuration"].ReplaceChild(sysDiagnosticsXmlNode,
                            configXmlNode[@"system.diagnostics"]);
                        configXml.Save(configFilePath);
                        flag = true;
                    }
                    else
                    {
                        var switchesXmlElem = configXml.CreateElement("switches");
                        const string newAddSwitchXmlStr = " <add name=\"TpTraceLevel\" value=\"4\" />";
                        switchesXmlElem.InnerXml = newAddSwitchXmlStr;
                        sysDiagnosticsXmlNode.AppendChild(switchesXmlElem);
                        configXml["configuration"].ReplaceChild(sysDiagnosticsXmlNode,
                            configXmlNode[@"system.diagnostics"]);
                        configXml.Save(configFilePath);
                        flag = true;
                    }
                }
                else
                {
                    var sysDiagXmlElem = configXml.CreateElement("system.diagnostics");
                    const string newSwitchXmlStr = "<switches>" +
                                                   " <add name=\"TpTraceLevel\" value=\"4\" />" +
                                                   "</switches>";
                    sysDiagXmlElem.InnerXml = newSwitchXmlStr;
                    configXml["configuration"].AppendChild(sysDiagXmlElem);
                    configXml.Save(configFilePath);
                    flag = true;
                }
            }
            catch (Exception e)
            {
                LogManager.WriteLog("Exception in EnableXMLTrace for config: " + configFilePath);
                LogManager.WriteLog("Exception details: " + e);
                return flag;
            }
            return flag;
        }

        public static bool DisableXmlTrace(string configFilePath)
        {
            // File existence check
            var flag = false;
            if (!File.Exists(configFilePath) || !File.Exists(configFilePath + BackupSuffix))
            {
                return flag;
            }

            try
            {
                // Check if configuration file can be rolled back from backup
                File.Copy(configFilePath + BackupSuffix, configFilePath, true);
                flag = true;
            }
            catch (Exception e)
            {
                LogManager.WriteLog("Exception in DisableXMLTrace for config: " + configFilePath);
                LogManager.WriteLog("Exception details: " + e);
                return flag;
            }

            return flag;
        }
    }
}
