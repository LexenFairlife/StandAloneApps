using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Data;

/// <summary>
/// Developed by Asad Sheikh in November 2020 
/// Source repository: https://github.com/fairlife-code/FairlifeApp
/// </summary>
public class Centerlines : MonoBehaviour
{
    //Element lists to populate centerline input scrollarea
    public List<GameObject> entryButtons;
    public List<Text> entryNames = new List<Text>();
    public List<Text> tooltips = new List<Text>();
    public List<InputField> measurementFields, textFields = new List<InputField>();
    //DB connection string - ideally find a way to do this aside from hard-coding the id & password
    public readonly string connectionString = @"Data Source = 172.16.5.217; user id = cipapp;password = bestinclass&1;Connection Timeout=10;";
    //specific elements used in the script
    public GameObject buttonContainer, panel, entries;
    public Button newCenterlineButton, saveCenterlineButton, submitCenterlineButton, confirmButton;
    public Dropdown siteDropdown, lineDropdown, eqDropdown, recDropdown;
    public InputField opInitialField, noteContainer, noteField, mainOpInitialField, mainNoteField;
    public Text initialsWarning, centerlineDataLabel;
    //local copy of master table, reduces SQL queries
    //site, list of lines
    public Dictionary<string, List<string>> lines = new Dictionary<string, List<string>>();
    //site+line, list of equipment
    public Dictionary<string, List<string>> equipment = new Dictionary<string, List<string>>();
    //site+line+equipment, list of recipes
    public Dictionary<string, List<string>> recipes = new Dictionary<string, List<string>>();
    //copy of current centerline entry
    public string[] centerline;
    //SQL table paths
    private readonly string masterTable = "[FairlifeMaster].[dbo].[CenterlineRecipes]";
    private readonly string centerlineTable = "[FairlifeOperations].[dbo].[CenterlineEntries]";
    private readonly string datapointTable = "[FairlifeOperations].[dbo].[CenterlineDatapoints]";

    public bool hasDatapoints = false; //flags to determine if an existing centerline is being edited (true) and if a centerline record has corresponding datapoint records (true)
    public bool[] datapointEdited;

    /// <summary>
    /// Runs on program startup, sets up scene for user to choose a centerline
    /// </summary>
    private void Awake()
    {
        //initial DB query for unique combinations of site/equipment/recipe
        FetchEquipmentOptions();
        FindElements();
        //populate dropdowns with initial values
        AddSites();
        AddObjectListeners();
        GetCenterlineSettings();
    }
    /// <summary>
    /// Pulls valid combinations of site, equipment, and recipe from the database to use in the dropdown lists
    /// </summary>
    public void FetchEquipmentOptions()
    {
        SqlDataReader reader;
        string query = "SELECT DISTINCT [Site],[Line],[Equipment],[Recipe]" +
            $"FROM {masterTable}";
        using (SqlConnection dbCon = new SqlConnection(connectionString))
        {
            dbCon.Open();
            SqlCommand cmd = new SqlCommand(query, dbCon);
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0) && !reader.IsDBNull(1) && !reader.IsDBNull(2) && !reader.IsDBNull(3))
                        {
                            string site = reader.GetSqlString(0).ToString();
                            string line = reader.GetSqlString(1).ToString();
                            string equip = reader.GetSqlString(2).ToString();
                            string recipe = reader.GetSqlString(3).ToString();
                            //populate dictionaries with unique values
                            if (lines.ContainsKey(site))
                            {
                                if (!lines[site].Contains(line))
                                {
                                    lines[site].Add(line);
                                }
                            }
                            else
                            {
                                lines.Add(site, new List<string> { line });
                            }

                            if (equipment.ContainsKey(site + line))
                            {
                                if (!equipment[site + line].Contains(equip))
                                {
                                    equipment[site + line].Add(equip);
                                }
                            }
                            else
                            {
                                equipment.Add(site + line, new List<string> { equip });
                            }
                            if (recipes.ContainsKey(site + line + equip))
                            {
                                recipes[site + line + equip].Add(recipe);
                            }
                            else
                            {
                                recipes.Add(site + line + equip, new List<string> { recipe });
                            }
                        }
                    }
                }
                reader.Close();
            }
            catch (SqlException e)
            {
                Debug.LogException(e);
            }
        };
    }
    /// <summary>
    /// Searches te master database for records matching the user's criteria, creates corresponding elements on the input scrollarea
    /// </summary>
    public IEnumerator PullNames()
    {
        SqlDataReader reader;
        string query = "SELECT [Datapoint],[Tooltip],[Measurement],[Text]" +
            $"FROM {masterTable}" +
            $"WHERE [Site] = '{centerline[1]}' AND [Line] = '{centerline[2]}' AND [Equipment] = '{centerline[3]}' AND [Recipe] = '{centerline[4]}'" +
            "ORDER BY [Datapoint]";
        //Store the values from the query in these lists
        List<string> names = new List<string>();
        List<string> tooltext = new List<string>();
        List<string> measurements = new List<string>();
        List<string> text = new List<string>();

        using (SqlConnection dbCon = new SqlConnection(connectionString))
        {
            dbCon.Open();
            SqlCommand cmd = new SqlCommand(query, dbCon);
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        names.Add(!reader.IsDBNull(0) ? reader.GetString(0) : "------");
                        tooltext.Add(!reader.IsDBNull(1) ? reader.GetString(1) : "N/A");
                        measurements.Add(!reader.IsDBNull(2) ? reader.GetDouble(2).ToString() : "");
                        text.Add(!reader.IsDBNull(3) ? reader.GetString(3) : "");
                    }
                }
                reader.Close();
            }
            catch (SqlException e)
            {
                Debug.LogException(e);
            }
        }
        yield return new WaitForSeconds(0);

        int halfList = names.Count / 2 + names.Count % 2;
        //generate and position the copies of entryButtons[0], fill all entryButtons with appropriate data
        if (tooltips.Count > 0)
        {
            tooltips[0].transform.parent.SetParent(entryButtons[0].transform);
        }
        for (int i = 0; i < names.Count; i++)
        {
            //skip creation step if the entry button already exists
            if (i >= entryButtons.Count)
            {
                GameObject tempGO = Instantiate(entryButtons[0], entryButtons[0].transform.parent, false);
                entryButtons.Add(tempGO);
            }
            //set button's position
            Vector3 v3 = new Vector3();
            if (i < halfList)
            {
                v3.x = -405;
                v3.y = 2714 - (60 * i);
            }
            else
            {
                v3.x = 405;
                v3.y = 2714 - (60 * (i - halfList));
            }
            entryButtons[i].transform.localPosition = v3;

            if (i >= entryNames.Count)
            {
                //add entry objects to list
                entryNames.Add(entryButtons[i].transform.Find("InputName").GetComponent<Text>());
                tooltips.Add(entryButtons[i].transform.Find("Tooltip/Text").GetComponent<Text>());
                measurementFields.Add(entryButtons[i].transform.Find("NumVal").GetComponent<InputField>());
                textFields.Add(entryButtons[i].transform.Find("TextVal").GetComponent<InputField>());
            }
            else
            {
                //relocate tooltips to match parent positions
                tooltips[i].transform.parent.SetParent(entryButtons[i].transform);
                tooltips[i].transform.parent.localPosition = new Vector3(0f, -30f);
            }
            //set initial values
            measurementFields[i].text = measurements[i];
            tooltips[i].text = tooltext[i];
            entryNames[i].text = names[i];
            textFields[i].text = text[i];
            measurementFields[i].placeholder.GetComponent<Text>().text = (measurements[i] == null || measurements[i].Equals("")) ? "N/A" : measurements[i];
            textFields[i].placeholder.GetComponent<Text>().text = text[i];
        }
        datapointEdited = new bool[names.Count];
        //remove extra entrybuttons
        for (int i = entryButtons.Count - 1; i >= names.Count; i--)
        {
            //delete game objects
            Destroy(entryButtons[i]);
            Destroy(tooltips[i].transform.parent.gameObject);
            //remove pointers
            entryButtons.RemoveAt(i);
            entryNames.RemoveAt(i);
            tooltips.RemoveAt(i);
            measurementFields.RemoveAt(i);
            textFields.RemoveAt(i);
        }

        for (int i = 0; i < tooltips.Count; i++)
        {
            //move tooltips in heirarchy to display above other entrybuttons
            tooltips[i].transform.parent.SetParent(entryButtons[0].transform.parent);
        }
        yield return null;
    }
    /// <summary>
    /// Finds elements in the project and sets local variables with their references
    /// </summary>
    private void FindElements()
    {
        //buttonContainer elements
        buttonContainer = transform.Find("Panel/Buttons").gameObject;
        confirmButton = buttonContainer.transform.Find("Confirm").GetComponent<Button>();
        siteDropdown = buttonContainer.transform.Find("Site Dropdown").GetComponent<Dropdown>();
        lineDropdown = buttonContainer.transform.Find("Line Dropdown").GetComponent<Dropdown>();
        eqDropdown = buttonContainer.transform.Find("Equipment Dropdown").GetComponent<Dropdown>();
        recDropdown = buttonContainer.transform.Find("Recipe Dropdown").GetComponent<Dropdown>();
        opInitialField = buttonContainer.transform.Find("Operator Initials").GetComponent<InputField>();
        noteContainer = buttonContainer.transform.Find("Notes").GetComponent<InputField>();
        noteField = noteContainer.transform.Find("ScrollArea").Find("Textbox").Find("Notes Input Field").GetComponent<CustomInputField>();
        initialsWarning = buttonContainer.transform.Find("Initials Warning").GetComponent<Text>();
        //main screen elements
        panel = transform.Find("Panel").gameObject;
        newCenterlineButton = panel.transform.Find("New Centerline Button").GetComponent<Button>();
        centerlineDataLabel = panel.transform.Find("Centerline Data Label").GetComponent<Text>();
        entries = panel.transform.Find("Entries").gameObject;
        mainOpInitialField = entries.transform.Find("Operator Initials").GetComponent<InputField>(); ;
        mainNoteField = entries.transform.Find("Notes").Find("ScrollArea").Find("Textbox").Find("Notes Input Field").GetComponent<CustomInputField>();
        saveCenterlineButton = entries.transform.Find("Save Centerline Button").GetComponent<Button>();
        submitCenterlineButton = entries.transform.Find("Submit Centerline Button").GetComponent<Button>();
    }
    /// <summary>
    /// Sets listeners to dropdowns and buttons in the program
    /// </summary>
    private void AddObjectListeners()
    {
        confirmButton.onClick.AddListener(delegate { ConfirmClick(); });
        newCenterlineButton.onClick.AddListener(delegate { GetCenterlineSettings(); });
        siteDropdown.onValueChanged.AddListener(delegate { UpdateLabel(siteDropdown); UpdateLines(); });
        lineDropdown.onValueChanged.AddListener(delegate { UpdateLabel(lineDropdown); UpdateEquipment(); });
        eqDropdown.onValueChanged.AddListener(delegate { UpdateLabel(eqDropdown); UpdateRecipes(); });
        recDropdown.onValueChanged.AddListener(delegate { UpdateLabel(recDropdown); });
        saveCenterlineButton.onClick.AddListener(delegate
        {
            InsertDatapointRecords();
            centerline[6] = mainNoteField.text;
            UpdateNotes();
            centerline[5] = mainOpInitialField.text;
            if (EditCenterlineRecord())
            {
                StartCoroutine(TurnGreen(saveCenterlineButton));
            }
            else
            {
                StartCoroutine(TurnRed(saveCenterlineButton));
            }
        });
        submitCenterlineButton.onClick.AddListener(delegate
        {
            InsertDatapointRecords();
            centerline[6] = mainNoteField.text;
            UpdateNotes();
            centerline[5] = mainOpInitialField.text;
            EditCenterlineRecord();
            GetCenterlineSettings();
        });

    }
    /// <summary>
    /// Takes the user to the initial selection screens to edit the current centerline's data via the dropdowns
    /// </summary>
    public void GetCenterlineSettings()
    {
        hasDatapoints = false;
        //enable dropdowns
        siteDropdown.gameObject.SetActive(true);
        lineDropdown.gameObject.SetActive(true);
        eqDropdown.gameObject.SetActive(true);
        recDropdown.gameObject.SetActive(true);
        opInitialField.gameObject.SetActive(true);
        noteContainer.gameObject.SetActive(true);
        //hide pop-up options
        initialsWarning.gameObject.SetActive(false);
        //reset fields for new entry
        centerline = new string[7];
        opInitialField.text = "";
        noteField.text = "";
        buttonContainer.SetActive(true);
    }
    /// <summary>
    ///populates the sites dropdown, calls UpdateEquipment
    /// </summary>
    public void AddSites()
    {
        List<string> keys = new List<string>();
        foreach (string k in lines.Keys)
        {
            keys.Add(k);
        }
        siteDropdown.AddOptions(keys);
        UpdateLabel(siteDropdown);
        UpdateLines();
    }

    /// <summary>
    ///populates the sites dropdown, calls UpdateEquipment
    /// </summary>
    public void UpdateLines()
    {
        lineDropdown.ClearOptions();
        lineDropdown.AddOptions(lines[siteDropdown.options[siteDropdown.value].text]);
        UpdateLabel(lineDropdown);
        UpdateEquipment();
    }
    /// <summary>
    /// Populates equipment dropdown with values corresponding to the selected site, calls UpdateRecipes
    /// </summary>
    public void UpdateEquipment()
    {
        eqDropdown.ClearOptions();
        eqDropdown.AddOptions(equipment[siteDropdown.options[siteDropdown.value].text + lineDropdown.options[lineDropdown.value].text]);
        UpdateLabel(eqDropdown);
        UpdateRecipes();
    }
    /// <summary>
    /// Populates recipe dropdown with values corresponding to the selected equipment and site, calls UpdateRecipes
    /// </summary>
    public void UpdateRecipes()
    {
        recDropdown.ClearOptions();
        recDropdown.AddOptions(recipes[siteDropdown.options[siteDropdown.value].text + lineDropdown.options[lineDropdown.value].text + eqDropdown.options[eqDropdown.value].text]);
        UpdateLabel(recDropdown);
    }

    public void UpdateLabel(Dropdown d)
    {
        Text label = d.transform.Find("Label").GetComponent<Text>();
        label.text = d.options[d.value].text;
    }
    /// <summary>
    /// Saves user selections on the buttoncontainer section, changes available input options
    /// </summary>
    public void ConfirmClick()
    {
        if (opInitialField.text.Length < 2)
        {
            initialsWarning.gameObject.SetActive(true);
        }
        else
        {
            initialsWarning.gameObject.SetActive(false);
            //save selections
            centerline[1] = siteDropdown.options[siteDropdown.value].text;
            centerline[2] = lineDropdown.options[lineDropdown.value].text;
            centerline[3] = eqDropdown.options[eqDropdown.value].text;
            centerline[4] = recDropdown.options[recDropdown.value].text;
            centerline[5] = opInitialField.text;
            centerline[6] = noteField.text ?? "";
            mainOpInitialField.text = centerline[5];
            mainNoteField.text = centerline[6];
            if (InsertCenterlineRecord())
            {
                StartCoroutine(PullNames());
                transform.Find("Panel/Buttons").gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("Record insert failed.");
            }
        }
    }
    /// <summary>
    /// Creates new centerline entry in the @centerlineTable
    /// </summary>
    /// <returns>True if record was successfully inserted, false otherwise.</returns>
    private bool InsertCenterlineRecord()
    {
        centerlineDataLabel.text = centerline[2] + " " + centerline[3] + " | " + centerline[4] + " | " + centerline[5];
        bool result = true;
        string query = $"INSERT INTO {centerlineTable} ([Site],[Line],[Equipment],[Recipe],[OperatorInitials],[DateTimeUTC],[Notes]) " +
            "OUTPUT INSERTED.ID VALUES (@site, @line, @equipment, @recipe, @opInitials, @UTCDate, @notes);";
        using (SqlConnection dbCon = new SqlConnection(connectionString))
        {
            dbCon.Open();
            using (SqlTransaction dbTrans = dbCon.BeginTransaction())
            {
                using (SqlCommand cmd = new SqlCommand(query, dbCon, dbTrans))
                {
                    cmd.Parameters.Add(new SqlParameter("@site", SqlDbType.VarChar, 4));
                    cmd.Parameters.Add(new SqlParameter("@line", SqlDbType.VarChar, 25));
                    cmd.Parameters.Add(new SqlParameter("@equipment", SqlDbType.VarChar, 25));
                    cmd.Parameters.Add(new SqlParameter("@recipe", SqlDbType.VarChar, 50));
                    cmd.Parameters.Add(new SqlParameter("@opInitials", SqlDbType.VarChar, 4));
                    cmd.Parameters.Add(new SqlParameter("@UTCDate", SqlDbType.DateTime));
                    cmd.Parameters.Add(new SqlParameter("@notes", SqlDbType.VarChar));

                    try
                    {
                        cmd.Parameters[0].Value = centerline[1];
                        cmd.Parameters[1].Value = centerline[2];
                        cmd.Parameters[2].Value = centerline[3];
                        cmd.Parameters[3].Value = centerline[4];
                        cmd.Parameters[4].Value = centerline[5];
                        cmd.Parameters[5].Value = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                        cmd.Parameters[6].Value = centerline[6];

                        centerline[0] = cmd.ExecuteScalar().ToString();

                        //if no exceptions occurred, commit to DB
                        dbTrans.Commit();
                    }
                    catch (Exception e)
                    {
                        result = false;
                        Debug.LogError(e);
                        Debug.Log(cmd.CommandText);
                        dbTrans.Rollback();
                    }
                    finally
                    {
                        dbTrans.Dispose();
                        dbCon.Close();
                        dbCon.Dispose();
                    }
                };
            };
        };
        return result;
    }

    /// <summary>
    /// Edits existing centerline entry in the @centerlineTable
    /// </summary>
    /// <returns>True if record was successfully updated, false otherwise.</returns>
    private bool EditCenterlineRecord()
    {
        centerlineDataLabel.text = centerline[2] + " " + centerline[3] + " | " + centerline[4] + " | " + centerline[5];
        bool result = true;
        string query = $"UPDATE  {centerlineTable} SET [Site] = @site, [Line] = @line, [Equipment] = @equipment, [Recipe] = @recipe, [OperatorInitials] = @opInitials, [DateTimeUTC] = @UTCDate," +
            $"[Notes] = @notes, [Compliance] = @compliance " +
            $"WHERE [ID] = @ID;";
        using (SqlConnection dbCon = new SqlConnection(connectionString))
        {
            dbCon.Open();
            using (SqlTransaction dbTrans = dbCon.BeginTransaction())
            {
                using (SqlCommand cmd = new SqlCommand(query, dbCon, dbTrans))
                {
                    cmd.Parameters.Add(new SqlParameter("@site", SqlDbType.VarChar, 4));
                    cmd.Parameters.Add(new SqlParameter("@line", SqlDbType.VarChar, 25));
                    cmd.Parameters.Add(new SqlParameter("@equipment", SqlDbType.VarChar, 25));
                    cmd.Parameters.Add(new SqlParameter("@recipe", SqlDbType.VarChar, 50));
                    cmd.Parameters.Add(new SqlParameter("@opInitials", SqlDbType.VarChar, 4));
                    cmd.Parameters.Add(new SqlParameter("@UTCDate", SqlDbType.DateTime));
                    cmd.Parameters.Add(new SqlParameter("@notes", SqlDbType.VarChar));
                    cmd.Parameters.Add(new SqlParameter("@Compliance", SqlDbType.Float));
                    cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));

                    try
                    {
                        cmd.Parameters[0].Value = centerline[1];
                        cmd.Parameters[1].Value = centerline[2];
                        cmd.Parameters[2].Value = centerline[3];
                        cmd.Parameters[3].Value = centerline[4];
                        cmd.Parameters[4].Value = centerline[5];
                        cmd.Parameters[5].Value = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                        cmd.Parameters[6].Value = centerline[6];
                        cmd.Parameters[7].Value = Math.Round((double)datapointEdited.Where(element => !element).Count() / datapointEdited.Count(), 4);
                        cmd.Parameters[8].Value = centerline[0];
                        //attempt to add changes to the database. If the number of records changed != 1, throw an exception
                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            throw new InvalidProgramException();
                        }
                        //else commit changes
                        dbTrans.Commit();
                    }
                    catch (Exception e)
                    {
                        result = false;
                        Debug.LogError(e);
                        Debug.Log(cmd.CommandText);
                        dbTrans.Rollback();
                    }
                    finally
                    {
                        dbTrans.Dispose();
                        dbCon.Close();
                        dbCon.Dispose();
                    }
                };
            };
        };
        return result;
    }
    /// <summary>
    /// Adds datapoint records to the @datapointTable. If records already exist for the related centerline, those records are deleted first.
    /// </summary>
    private void InsertDatapointRecords()
    {
        if (hasDatapoints) { DeleteDatapoints(); }
        using (SqlConnection dbCon = new SqlConnection(connectionString))
        {
            dbCon.Open();
            using (SqlTransaction dbTrans = dbCon.BeginTransaction())
            {
                using (SqlCommand cmd = dbCon.CreateCommand())
                {
                    cmd.Transaction = dbTrans;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = $"INSERT INTO {datapointTable} ([Datapoint], [CenterlineEntries_ID], [Measurement], [Text]) VALUES (@datapoint, @centerlines_id, @measurement, @text);";
                    cmd.Parameters.Add(new SqlParameter("@datapoint", SqlDbType.VarChar, 50));
                    cmd.Parameters.Add(new SqlParameter("@centerlines_id", SqlDbType.Int));
                    cmd.Parameters.Add(new SqlParameter("@measurement", SqlDbType.Float));
                    cmd.Parameters.Add(new SqlParameter("@text", SqlDbType.VarChar, 50));
                    try
                    {
                        for (int i = 0; i < entryButtons.Count; i++)
                        {
                            cmd.Parameters[0].Value = entryNames[i].text;
                            cmd.Parameters[1].Value = centerline[0];
                            cmd.Parameters[2].Value = measurementFields[i].text == "" ? DBNull.Value : (object)measurementFields[i].text;
                            cmd.Parameters[3].Value = textFields[i].text;
                            //if the number of records affected by the command != 1, throw exception
                            if (cmd.ExecuteNonQuery() != 1)
                            {
                                throw new InvalidProgramException();
                            }
                        }
                        dbTrans.Commit();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        Debug.Log(cmd.CommandText);
                        dbTrans.Rollback();
                        throw;
                    }
                };
            };
        };
        hasDatapoints = true;
    }
    /// <summary>
    /// Deletes the current centerline from the @centerlineTable in the database, then deletes linked records in the @datapointTable
    /// </summary>
    /*private void DeleteCenterline()
    {
        if (hasDatapoints)
        {
            DeleteDatapoints();
        }
        string query = $"DELETE FROM {centerlineTable} WHERE [ID] = @ID";
        using (SqlConnection dbCon = new SqlConnection(connectionString))
        {
            dbCon.Open();
            using (SqlTransaction dbTrans = dbCon.BeginTransaction())
            {
                using (SqlCommand cmd = new SqlCommand(query, dbCon, dbTrans))
                {
                    cmd.Parameters.Add("@ID", SqlDbType.Int);
                    try
                    {
                        cmd.Parameters[0].Value = centerline[0];
                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            throw new InvalidProgramException();
                        }
                        dbTrans.Commit();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        Debug.Log(cmd.CommandText);
                        throw;
                    }
                    finally
                    {
                        dbTrans.Dispose();
                        dbCon.Close();
                        dbCon.Dispose();
                    }
                };
            };
        };
    }*/
    /// <summary>
    /// Deletes datapoint records from the @datapointTable
    /// </summary>
    private void DeleteDatapoints()
    {
        string query = $"DELETE FROM {datapointTable} WHERE [CenterlineEntries_ID] = @ID";
        using (SqlConnection dbCon = new SqlConnection(connectionString))
        {
            dbCon.Open();
            using (SqlTransaction dbTrans = dbCon.BeginTransaction())
            {
                using (SqlCommand cmd = new SqlCommand(query, dbCon, dbTrans))
                {
                    cmd.Parameters.Add("@ID", SqlDbType.Int);
                    try
                    {
                        cmd.Parameters[0].Value = centerline[0];
                        cmd.ExecuteNonQuery();
                        dbTrans.Commit();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        Debug.Log(cmd.CommandText);
                    }
                    finally
                    {
                        dbTrans.Dispose();
                        dbCon.Close();
                        dbCon.Dispose();
                    }
                };
            };
        };
        hasDatapoints = false;
    }
    /// <summary>
    /// Detects if datapoint values differ from the master recipe values, returns user to the note editing screen with appropriate alerts
    /// </summary>
    private void UpdateNotes()
    {
        //Compare entry to master settings and update notes with prompts
        for (int i = 0; i < entryButtons.Count; i++)
        {
            if ((measurementFields[i].text != (measurementFields[i].placeholder.GetComponent<Text>().text == "N/A" ? "" : measurementFields[i].placeholder.GetComponent<Text>().text)
            || textFields[i].text != textFields[i].placeholder.GetComponent<Text>().text))
            {
                if (!datapointEdited[i])
                {
                    datapointEdited[i] = true;
                    centerline[6] += $"\n{entryNames[i].text} adj. due to: ";
                }
            }
            else
            {
                datapointEdited[i] = false;
            }
        }
        mainNoteField.text = centerline[6];
    }
    /// <summary>
    /// Turns an img element green for 1 second, then reverts its color
    /// </summary>
    /// <param name="b">button element which has a parent gameobject containing an Image</param>
    private IEnumerator TurnGreen(Button b)
    {
        //commented out saving original color and returning to that due to rapid clicks saving the green color
        //this entire method is probably not the best way to do this, update if anyone has a better approach
        Image img = b.GetComponentInParent<Image>();
        //Color prev = img.color;
        img.color = new Color(0.28f, 1, .36f, 1);
        yield return new WaitForSecondsRealtime(0.3f);
        img.color = new Color(0.505f, 0.842f, 1, 1);
        //img.color = prev;
    }
    /// <summary>
    /// Turns an img element green for 1 second, then reverts its color
    /// </summary>
    /// <param name="b">button element which has a parent gameobject containing an Image</param>
    private IEnumerator TurnRed(Button b)
    {
        //commented out saving original color and returning to that due to rapid clicks saving the green color
        //this entire method is probably not the best way to do this, update if anyone has a better approach
        Image img = b.GetComponentInParent<Image>();
        //Color prev = img.color;
        img.color = new Color(0.98f, .18f, .22f, 1);
        yield return new WaitForSecondsRealtime(0.3f);
        img.color = new Color(0.505f, 0.842f, 1, 1);
        //img.color = prev;
    }
}
