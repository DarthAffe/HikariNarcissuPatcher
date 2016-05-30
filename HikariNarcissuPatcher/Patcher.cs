using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace HikariNarcissuPatcher
{
    public class Patcher
    {
        #region Constants

        private static readonly IEnumerable<string> POSSIBLE_INSTALL_PATHS = new List<string>
        {
            @"C:\Program Files (x86)\Steam\SteamApps\common\narcissu2",
            @"C:\Program Files\Steam\SteamApps\common\narcissu2"
        };

        private static readonly IEnumerable<string> POSSIBLE_STEAM_PATHS = new List<string>
        {
            @"C:\Program Files (x86)\Steam",
            @"C:\Program Files\Steam"
        };

        private const string EXE_NARCISSU = "narci2.exe";
        private const string EXE_STEAM = "steam.exe";
        private const string STEAM_PARAMS = "-applaunch 264380";

        private const string RESOURCE_ORIG_FOLDER = @"pack://application:,,,/Files/Original/";
        private const string RESOURCE_HIKARI_FOLDER = @"pack://application:,,,/Files/Hikari/";
        private static readonly IEnumerable<string> PATCH_FILES = new List<string>
        {
            "0.utf",
            "arc1.nsa"
        };

        private static readonly IEnumerable<string> NEW_FILES = new List<string>
        {
            "Hikari Readme.txt"
        };

        #endregion

        #region Properties & Fields

        private string _gamePath;
        private string _steamPath;

        #endregion

        #region Methods

        public void StartGame()
        {
            if (_steamPath == null || !Directory.Exists(_steamPath) || !File.Exists(Path.Combine(_steamPath, EXE_STEAM))) return;

            try
            {
                Process.Start(Path.Combine(_steamPath, EXE_STEAM), STEAM_PARAMS);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.Replace("%1", "'" + Path.Combine(_gamePath, EXE_NARCISSU) + "'"), "Beim Starten des Spiels ist ein Fehler aufgetreten");
            }
        }

        public void Install()
        {
            if (_gamePath == null || !Directory.Exists(_gamePath)) return;

            try
            {
                CopyPatchFiles(RESOURCE_HIKARI_FOLDER);
                InstallNewFiles(RESOURCE_HIKARI_FOLDER);

                if (_steamPath != null)
                {
                    MessageBoxResult result = MessageBox.Show("Der Patch wurde erfolgreich installiert, soll das Spiel gestartet werden?", "Patch erfolgreich installiert", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                        StartGame();
                }
                else
                    MessageBox.Show("Der Patch wurde erfolgreich installiert.", "Patch erfolgreich installiert");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Beim Installieren des Patches ist ein Fehler aufgetreten");
            }
        }

        public void Uninstall()
        {
            if (_gamePath == null || !Directory.Exists(_gamePath)) return;

            try
            {
                CopyPatchFiles(RESOURCE_ORIG_FOLDER);
                RemoveNewFiles(_gamePath);
                MessageBox.Show("Der Patch wurde erfolgreich deinstalliert.", "Patch erfolgreich deinstalliert");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Beim Deinstallieren des Patches ist ein Fehler aufgetreten");
            }
        }

        private void CopyPatchFiles(string resourceFolder)
        {
            foreach (string file in PATCH_FILES)
            {
                using (Stream stream = Application.GetResourceStream(new Uri(resourceFolder + file)).Stream)
                {
                    using (FileStream fileStream = File.Create(Path.Combine(_gamePath, file)))
                    {
                        stream.CopyTo(fileStream);
                    }
                }
            }
        }

        private void InstallNewFiles(string resourceFolder)
        {
            foreach (string file in NEW_FILES)
            {
                using (Stream stream = Application.GetResourceStream(new Uri(resourceFolder + file)).Stream)
                {
                    using (FileStream fileStream = File.Create(Path.Combine(_gamePath, file)))
                    {
                        stream.CopyTo(fileStream);
                    }
                }
            }
        }

        private void RemoveNewFiles(string gamePath)
        {
            foreach (string file in NEW_FILES)
            {
                string path = Path.Combine(gamePath, file);
                if (File.Exists(path))
                    File.Delete(path);
            }
        }

        public bool InitializeGamePath()
        {
            foreach (string possiblePath in POSSIBLE_INSTALL_PATHS)
                if (Directory.Exists(possiblePath))
                    if (File.Exists(Path.Combine(possiblePath, EXE_NARCISSU)))
                    {
                        _gamePath = possiblePath;
                        return true;
                    }
            return AskUserForPath();
        }

        public bool InitializeSteamPath()
        {
            foreach (string possiblePath in POSSIBLE_STEAM_PATHS)
                if (Directory.Exists(possiblePath))
                    if (File.Exists(Path.Combine(possiblePath, EXE_STEAM)))
                    {
                        _steamPath = possiblePath;
                        return true;
                    }

            _steamPath = null;
            return false;
        }

        private bool AskUserForPath()
        {
            DialogResult result;
            do
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog
                {
                    Description = "Bitte wähle dein Narcissu-Installationsverzeichnis (das mit der 'narci2.exe').\r\n(Dieses sollte sich normalerweise in deinem Stream-Verzeichnis befinden.)"
                };
                result = dialog.ShowDialog();

                if (result == DialogResult.OK && File.Exists(Path.Combine(dialog.SelectedPath, EXE_NARCISSU)))
                {
                    _gamePath = dialog.SelectedPath;
                    return true;
                }
            } while (result == DialogResult.OK);

            return false;
        }

        #endregion
    }
}
