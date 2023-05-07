using MusicXml;
using MusicXml.Domain;
using System.Runtime.InteropServices;
using System.Xml;

namespace MusicXMLMiniApp
{
    internal class Program
    {
        private static Score Score;
        private static string selectedSongStr;
        static void Main(string[] args)
        {
            bool endApp = false;
            Console.WriteLine("Console Simple Music XML in C#\r");
            Console.WriteLine("------------------------\n");

            while (!endApp)
            {
                // Ask the user to choose an operator.
                Console.WriteLine("Choose an operator from the following list:");
                Console.WriteLine("\t1 - Select a song to process");
                Console.WriteLine("\t2 - Change Tempo");
                Console.WriteLine("\t3 - Change Transpose");
                Console.WriteLine("\t4 - Change Dynamics");
                Console.WriteLine("\t5 - Add Chord and Rhythm");
                Console.WriteLine("\t6 - Show Lyric");
                Console.Write("Your option? ");

                int op = int.Parse(Console.ReadLine());

                if (op != 1 && string.IsNullOrEmpty(selectedSongStr))
                {
                    Console.WriteLine("No song to process. Please option '1 - Select a song to process' first. Thank you.\n");
                    continue;
                }

                try
                {
                    switch (op)
                    {
                        case 1:
                            SelectSong();
                            break;
                        case 2:
                            ChangeTempo();
                            break;
                        case 3:
                            ChangeTranspose();
                            break;
                        case 4:
                            ChangeDynamics();
                            break;
                        case 5:
                            AddChordandRhythm();
                            break;
                        case 6:
                            ShowLyric();
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("The process has some errors: " + ex.ToString());
                }

                Console.WriteLine("------------------------\n");

                // Wait for the user to respond before closing.
                Console.Write("Press 'n' and Enter to close the app, or press any other key and Enter to continue: ");
                if (Console.ReadLine() == "n") endApp = true;

                Console.WriteLine("\n"); // Friendly linespacing.
            }

            Console.ReadLine();
        }

        private static void SelectSong()
        {
            DirectoryInfo d = new DirectoryInfo(@"TestData");
            FileInfo[] Files = d.GetFiles("*.xml");
            Dictionary<int, string> SongList = new Dictionary<int, string>();
            Console.WriteLine("Current song list:");
            var counter = 0;
            foreach (FileInfo file in Files)
            {
                counter++;
                Console.WriteLine($"{counter}: {file.Name}");
                SongList.Add(counter, file.Name);
            }

            Console.Write("Please choose a song: ");
            var selectedSong = int.Parse(Console.ReadLine());
            selectedSongStr = SongList.GetValueOrDefault(selectedSong);
            Console.WriteLine($"You selected: {selectedSong} - {selectedSongStr}");
            try
            {
                Score = MusicXmlParser.GetScore($"TestData/{selectedSongStr}");
                if (Score != null)
                    Console.WriteLine("Your song is ready.");
                else
                    Console.WriteLine("The file has problem, please double check.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("The file has problem, please double check." + ex.ToString());
            }
        }

        private static void ChangeTempo()
        {
            if (!string.IsNullOrEmpty(selectedSongStr))
            {
                int tempoValueChanged = 0;
                var filePath = $"TestData/{selectedSongStr}";

                // Load the MusicXML file
                XmlDocument doc = LoadFile(filePath);

                // Find the node that you want to change
                XmlNode beatUnitNode = doc.SelectSingleNode("/score-partwise/part/measure/direction/direction-type/metronome/per-minute");
                XmlNode perminuteNode = doc.SelectSingleNode("/score-partwise/part/measure/direction/sound");
                var snapshotValue = string.Empty;
                if (beatUnitNode != null && perminuteNode != null)
                {
                    Console.Write("Please enter tempo value you want to change: ");
                    tempoValueChanged = int.Parse(Console.ReadLine());
                    snapshotValue = beatUnitNode.InnerText;
                    beatUnitNode.InnerText = tempoValueChanged.ToString();
                    perminuteNode.Attributes["tempo"].Value = tempoValueChanged.ToString();
                }
                else
                {
                    Console.WriteLine("Cannot find beat-unit and per-minute node. Tempo cannot be changed successfully.");
                }

                // Save the modified document
                SaveFile(doc, filePath);
                Console.WriteLine($"Tempo was changed from {snapshotValue} to {tempoValueChanged}.");
            }
        }

        /// <summary>
        /// <transpose>
        /// <diatonic>-4</diatonic>
        /// <chromatic>-7</chromatic>
        /// </transpose>
        /// </summary>
        private static void ChangeTranspose()
        {
            if (!string.IsNullOrEmpty(selectedSongStr))
            {
                var filePath = $"TestData/{selectedSongStr}";
                // Load the MusicXML file
                XmlDocument doc = LoadFile(filePath);

                // Find the node that you want to change
                XmlNode transposeNode = doc.SelectSingleNode("/score-partwise/part/measure/attributes/transpose");
                if (transposeNode != null)
                {
                    XmlNode diatonic = transposeNode.SelectSingleNode("diatonic");
                    if (diatonic != null)
                    {
                        Console.WriteLine("Do you want to change dianotic value [y/n]?");
                        if (Console.ReadLine().ToLower() == "y")
                        {
                            Console.Write("Please enter diatonic changed value: ");
                            int diatonicValue = int.Parse(Console.ReadLine());
                            int snapshotDiatonicValue = int.Parse(diatonic.InnerText);

                            diatonic.InnerText = diatonicValue.ToString();
                            Console.WriteLine($"Diatonic was changed from {snapshotDiatonicValue} to {diatonicValue}.");
                        }
                    }
                    else { Console.Write("This song does not contain Diatonic value."); }

                    XmlNode chromatic = transposeNode.SelectSingleNode("chromatic");
                    if (chromatic != null)
                    {
                        Console.WriteLine("Do you want to change chromatic value [y/n]?");
                        if (Console.ReadLine().ToLower() == "y")
                        {
                            Console.Write("Please enter chromatic changed value: ");
                            int chromaticValue = int.Parse(Console.ReadLine());
                            int snapshotchromaticValue = int.Parse(chromatic.InnerText);

                            chromatic.InnerText = chromaticValue.ToString();
                            Console.WriteLine($"Chromatic was changed from {snapshotchromaticValue} to {chromaticValue}.");
                        }
                    }
                    else { Console.Write("This song does not contain Chromatic value."); }
                }
                else
                {
                    Console.Write("This song does not contain Transpose.");
                }

                // Save the modified document
                SaveFile(doc, filePath);
            }
        }

        /// <summary>
        /// <direction placement="below">
        ///     <direction-type>
        ///         <dynamics default-x="129" default-y="-75">
        ///	            <pp/>
        ///         </dynamics>
        ///     </direction-type>
        ///     <staff>1</staff>
        ///     <sound dynamics = "40" />
        /// </ direction>
        /// </summary>
        private static void ChangeDynamics()
        {
            if (!string.IsNullOrEmpty(selectedSongStr))
            {
                var filePath = $"TestData/{selectedSongStr}";
                // Load the MusicXML file
                XmlDocument doc = LoadFile(filePath);

                // Find the node that you want to change
                XmlNode dynamicsNode = doc.SelectSingleNode("/score-partwise/part/measure/direction/direction-type/dynamics");
                if (dynamicsNode != null)
                {
                    if (!string.IsNullOrEmpty(dynamicsNode.InnerText))
                    {
                        Console.WriteLine("Do you want to change dynamics value [y/n]?");
                        if (Console.ReadLine().ToLower() == "y")
                        {
                            Console.Write("Please enter dynamics changed value: ");
                            var dynamicsValue = Console.ReadLine();
                            var snapshotDynamicsValue = dynamicsNode.InnerText;
                            if (!string.IsNullOrEmpty(dynamicsValue))
                            {
                                dynamicsValue = $"<{dynamicsValue}/>";
                                dynamicsNode.InnerText = dynamicsValue;
                                Console.WriteLine($"Dynamics was changed from {snapshotDynamicsValue} to {dynamicsValue}.");
                            }
                        }
                    }

                    // Has 2 attributes default-x and default-y
                    foreach (XmlAttribute att in dynamicsNode.Attributes)
                    {
                        if (att.Name == "default-x")
                        {
                            Console.WriteLine("Do you want to change default-x value [y/n]?");
                            if (Console.ReadLine().ToLower() == "y")
                            {
                                Console.Write("Please enter default-x changed value: ");
                                int defaultXValue = int.Parse(Console.ReadLine());
                                int snapshotDefaultXValue = int.Parse(att.Value);
                                att.InnerText = defaultXValue.ToString();
                                Console.WriteLine($"Default-x attribute was changed from {snapshotDefaultXValue} to {defaultXValue}.");
                            }
                            continue;
                        }
                        if (att.Name == "default-y")
                        {
                            Console.WriteLine("Do you want to change default-y value [y/n]?");
                            if (Console.ReadLine().ToLower() == "y")
                            {
                                Console.Write("Please enter default-y changed value: ");
                                int defaultYValue = int.Parse(Console.ReadLine());
                                int snapshotDefaultYValue = int.Parse(att.Value);
                                att.InnerText = defaultYValue.ToString();
                                Console.WriteLine($"Default-y attribute was changed from {snapshotDefaultYValue} to {defaultYValue}.");
                            }
                            continue;
                        }
                    }
                }
                else
                {
                    Console.Write("This song does not contain Dynamics.");
                    return;
                }

                XmlNode staffNode = doc.SelectSingleNode("/score-partwise/part/measure/direction/staff");
                if (staffNode != null)
                {
                    Console.WriteLine("Do you want to change staff value [y/n]?");
                    if (Console.ReadLine().ToLower() == "y")
                    {
                        Console.Write("Please enter staff changed value: ");
                        var staffValue = int.Parse(Console.ReadLine());
                        var snapshotStaffValue = int.Parse(staffNode.InnerText);
                        staffNode.InnerText = staffValue.ToString();
                        Console.WriteLine($"Staff was changed from {snapshotStaffValue} to {staffValue}.");
                    }
                }

                var soundNode = doc.SelectNodes("/score-partwise/part/measure/direction/sound");

                if (soundNode != null)
                {
                    foreach (XmlNode node in soundNode)
                    {
                        foreach (XmlAttribute xmlAttribute in node.Attributes)
                        {
                            if (xmlAttribute.Name == "dynamics")
                            {
                                Console.WriteLine("Do you want to dynamics attribute value of sound [y/n]?");
                                if (Console.ReadLine().ToLower() == "y")
                                {
                                    Console.Write("Please enter dynamics attribute changed value: ");
                                    var dynamicsValue = int.Parse(Console.ReadLine());
                                    var snapshotDynamicsValue = int.Parse(xmlAttribute.Value);
                                    xmlAttribute.Value = dynamicsValue.ToString();
                                    Console.WriteLine($"Dynamics attribute of sound was changed from {snapshotDynamicsValue} to {dynamicsValue}.");
                                }

                                continue;
                            }
                        }
                    }
                }

                SaveFile(doc, filePath);
            }
        }

        private static void AddChordandRhythm()
        {
            // TODO
            Console.WriteLine("To do later.");
        }

        private static void ShowLyric()
        {
            if (Score != null)
            {
                var lyricStr = string.Empty;
                foreach (var path in Score.Parts)
                {
                    foreach (var measure in path.Measures)
                    {
                        foreach (var lyric in measure.MeasureElements)
                        {
                            if (lyric.Element != null)
                            {
                                var note = lyric.Element as Note;
                                if (note != null && note.Lyric != null)
                                {
                                    lyricStr += note.Lyric.Text + " ";
                                }
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(lyricStr))
                {
                    SetConsoleOutputCP(65001);
                    Console.OutputEncoding = System.Text.Encoding.UTF8;
                    Console.WriteLine("The lyric is: " + lyricStr.Substring(0, lyricStr.Length - 1));
                }
                else { Console.WriteLine("The lyric is empty."); }
            }
        }

        private static XmlDocument LoadFile(string filePath)
        {
            // Load the MusicXML file
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            return doc;
        }

        private static void SaveFile(XmlDocument doc, string filePath)
        {
            doc.Save(filePath);
        }

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleOutputCP(uint wCodePageID);
    }
}
