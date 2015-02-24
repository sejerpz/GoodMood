﻿/*
 * Copyright 2015 Andrea Del Signore sejerpz@gmail.com
 */

using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoodMood
{
    public partial class FormAbout : MetroForm
    {
        public FormAbout()
        {
            InitializeComponent();
            Defaults();
        }

        private void Defaults()
        {
            this.Text = Application.ProductName;
            metroLabelVersion.Text = string.Format("version {0}", Application.ProductVersion);
            metroLabelCopyRight.Text = string.Format("copyright © 2015 by {0}", Application.CompanyName);
            metroLinkWebSite.Text = "GoodMood Web Site";

            metroLinkCheckUpdates.Visible = ApplicationDeployment.IsNetworkDeployed;
        }

        private void FormAbout_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void metroLinkWebSite_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://sejerpz.github.io/goodmood/");
        }

        private void metroLinkCheckUpdates_Click(object sender, EventArgs e)
        {
            CheckAndInstallUpdate();
        }

        private void CheckAndInstallUpdate()
        {
            UpdateCheckInfo info = null;

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;

                try
                {
                    info = ad.CheckForDetailedUpdate();
                }
                catch (DeploymentDownloadException dde)
                {
                    Interaction.Warning ("The new version of the application cannot be downloaded at this time. \n\nPlease check your network connection, or try again later. Error: " + dde.Message);
                    return;
                }
                catch (InvalidDeploymentException ide)
                {
                    Interaction.Warning("Cannot check for a new version of the application. The ClickOnce deployment is corrupt. Please redeploy the application and try again. Error: " + ide.Message);
                    return;
                }
                catch (InvalidOperationException ioe)
                {
                    Interaction.Info("This application cannot be updated. It is likely not a ClickOnce application. Error: " + ioe.Message);
                    return;
                }

                if (info.UpdateAvailable)
                {
                    Boolean doUpdate = true;

                    if (!info.IsUpdateRequired)
                    {
                        var  result = Interaction.Query("An update is available. Would you like to update the application now?", "Updated now", "No");
                        if (result != FormDialog.MessageResult.Ok)
                        {
                            doUpdate = false;
                        }
                    }
                    else
                    {
                        // Display a message that the app MUST reboot. Display the minimum required version.
                        Interaction.Info("This application has detected a mandatory update from your current " +
                            "version to version " + info.MinimumRequiredVersion.ToString() +
                            ". The application will now install the update and restart.",
                            "Update now");
                    }

                    if (doUpdate)
                    {
                        try
                        {
                            ad.Update();
                            Interaction.Info("The application has been upgraded, and will now restart.");
                            Application.Restart();
                        }
                        catch (DeploymentDownloadException ex)
                        {
                            Interaction.Error(ex);
                        }
                    }
                }
            }
        }
    }
}
