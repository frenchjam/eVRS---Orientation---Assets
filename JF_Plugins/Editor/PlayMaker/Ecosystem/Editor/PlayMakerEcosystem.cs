using System;
using System.Text.RegularExpressions;

using System.Collections;
using System.Collections.Generic;

using System.IO;

using UnityEngine;
using UnityEditor;

using HutongGames;
using HutongGames.Editor;
using HutongGames.PlayMaker;
using HutongGames.PlayMakerEditor;


public class PlayMakerEcosystemWindowEditor : EditorWindow {

	static private bool _disclaimer_pass = false;

	static private string __REST_URL_BASE__ = "http://www.fabrejean.net/projects/playmaker_ecosystem/";

	//static private string RepositoryPath = "jeanfabre/PlayMakerCustomActions";//"pbhogan/InControl";

	string searchString = "";
//	string searchStringFeedback = "";

	string rawSearchResult="";

//	string searchUrlBase = "https://api.github.com/search/";

	int resultCount =0;

	WWW wwwSearch;


	string selectedAction;

	Hashtable searchResultHash;

	Hashtable[] resultItems;


	Dictionary<string,string> downloadsLUT;

	Dictionary<string,Hashtable> itemsLUT;

	List<WWW> downloads;
	
	private SearchBox searchBox;


//	private RssReader rssFeed;


	#region Editor window properties
//	Vector2 mousePos;
	Vector2 _scroll;
	
	private string editorPath;
	private string editorSkinPath;
	
	private GUISkin editorSkin;
	
	private Vector2 lastMousePosition;
	private int mouseOverRowIndex;
	private Rect[] rowsArea;
	
	private Texture2D bg;

//	private GUIStyle GUIStyleArrowInBuildSettings;

	#endregion
	
	// Add menu named "My Window" to the Window menu
	[MenuItem ("PlayMaker/Addons/Ecosystem &e")]
	static void Init () {

		RefreshDisclaimerPref();

		// Get existing open window or if none, make a new one:
		PlayMakerEcosystemWindowEditor window = (PlayMakerEcosystemWindowEditor)EditorWindow.GetWindow (typeof (PlayMakerEcosystemWindowEditor));

		window.minSize = new Vector2(250,100);

	}


	#region Disclaimer

	static string _disclaimerPass_key = "Ecosystem Disclaimer Pass";
	static string _disclaimer_label = "By using this ecosystem, you understand that you will be able to download content (raw scripts and Unity packages) from various online sources and install them on your computer within this project. In doubt, do not use this and get in touch with us to learn more before you work with it.\nTips, make use of online repositories and keep regular backup of your projects.";

	static void RefreshDisclaimerPref()
	{
		// Debug.Log(_disclaimerPass_key+"-"+Application.dataPath);
		_disclaimer_pass = EditorPrefs.GetBool(_disclaimerPass_key+"-"+Application.dataPath);
	}
	
	void OnGUI_Disclaimer()
	{
		GUILayout.BeginVertical();
			GUILayout.Label(" -- Disclaimer -- ");
			GUILayout.Space(10);
			GUILayout.Label(_disclaimer_label);
			GUILayout.Space(10);	
			if ( GUILayout.Button("Learn more (online help)","Button") )
			{
				Application.OpenURL ("https://hutonggames.fogbugz.com/default.asp?W1181");
			}
			GUILayout.Space(5);
			if ( GUILayout.Button("Use the ecosystem!","Button") )
			{
				_disclaimer_pass = true;
				//Debug.Log(_disclaimerPass_key+"-"+Application.dataPath);
				EditorPrefs.SetBool(_disclaimerPass_key+"-"+Application.dataPath,true);
				
			}
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		if( GUILayout.Button("","Label UI Kit Credit") )
		{
			Application.OpenURL("http://www.killercreations.co.uk/volcanic-ui-kit.php");
		}
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("","Label Jean Fabre Url") )
		{
			Application.OpenURL("http://hutonggames.com/playmakerforum/index.php?action=profile;u=33");
		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();

	}

	#endregion

	private void OnFocus()
	{
		InitSearchBox();
		searchBox.Focus();

		RefreshDisclaimerPref();

		Repaint();
	}



	private void InitSearchBox()
	{
		if (searchBox == null)
		{
			searchBox = new SearchBox(this) {SearchModes = new[] {"All","Actions","Addons","Templates","Samples"}, HasPopupSearchModes = false};
			searchBox.SearchChanged += UpdateSearchResults;
			searchBox.Focus();
		}
	}

	private void UpdateSearchResults()
	{
		searchString = searchBox.SearchString;
	}

	private WWW _test;

	void OnGUI () {

		/*
		if (rssFeed==null)
		{
			rssFeed = RssReader.Create("http://feeds.feedburner.com/PlaymakerEcosystem");
		}
		*/
		if (FsmEditor.Instance == null)
		{
			GUILayout.Label("PlayMaker Editor must be opened for the ecosystem to work");
			if (GUILayout.Button("Open PlayMaker Editor"))
			{
				EditorApplication.ExecuteMenuItem("PlayMaker/PlayMaker Editor");
			}
			return;
		}


		try
		{
			FsmEditorGUILayout.Equals(null,null);
		}catch
		{
			GUILayout.Label("Project assets being processed, please wait");
			return;
		}


		wantsMouseMove = true;

		/*
		if (Event.current.isMouse)
		{
			mousePos = Event.current.mousePosition;
		}
*/
		if (Event.current.type == EventType.MouseMove)
		{
			Repaint ();
		}

		// set up the skin if not done yet.
		if (editorSkin==null)
		{
			editorSkin = JF_EditorUtils.GetGuiSkin("VolcanicGuiSkin",out editorSkinPath);
			bg = (Texture2D)(Resources.LoadAssetAtPath(editorSkinPath+"images/bg.png",typeof(Texture2D))); // Get the texture manually as we have some trickes for bg tiling
			
			//GUIStyleArrowInBuildSettings = editorSkin.FindStyle("Help Arrow 90 degree");
			
		}
		
		// draw the bg properly. Haven't found a way to do it with guiskin only
		if(bg!=null)
		{
			if (bg.wrapMode!= TextureWrapMode.Repeat)
			{
				bg.wrapMode = TextureWrapMode.Repeat;
			}
			GUI.DrawTextureWithTexCoords(new Rect(0,0,position.width,position.height),bg,new Rect(0, 0, position.width / bg.width, position.height / bg.height));
		}



		if(!Application.isPlaying && _disclaimer_pass)
		{
			OnGUI_ToolBar();
		}

		GUI.skin = editorSkin; // should design the scroll widgets so that it can be matching the skin.

		
		if (!_disclaimer_pass)
		{
			OnGUI_Disclaimer();
			return;
		}

		if (Application.isPlaying)
		{
			GUILayout.Label("Application is playing. Saves performances to not process anything during playback.");
			return ;
		}

		/*
		GUILayout.Label(searchStringFeedback);

		if(wwwSearch!=null)
		{
			GUILayout.Label("Searching...");
		}else{
			GUILayout.Label("");
		}
*/

		OnGUI_ActionList();

		OnGUI_BottomPanel();





		// detect mouse over top area of the browser window to toggle the toolbar visibility if required
		if ( rowsArea!=null && Event.current.type == EventType.Repaint)
		{
			if (lastMousePosition!= Event.current.mousePosition)
			{
				int topDelta = 20;// ShowToolBar || ShowHelp || !DiscreteTooBar ? 20:0;

				int j=0;
				mouseOverRowIndex = -1;
				foreach(Rect _row in rowsArea)
				{
					Rect _temp = _row;
					_temp.x = _temp.x  -_scroll.x;
					_temp.y = _temp.y + topDelta -_scroll.y;
					if (_temp.Contains(Event.current.mousePosition))
					{
						mouseOverRowIndex = j;
						break;
					}
					j++;
				}
			}
			lastMousePosition = Event.current.mousePosition;
		}

		// User click on a row.
		if (Event.current.type == EventType.MouseDown && mouseOverRowIndex!=-1)
		{

		//	Debug.Log("User clicked on row "+mouseOverRowIndex);
		/*	UnityEngine.Object _scene =	AssetDatabase.LoadMainAssetAtPath("Assets/"+scenes[mouseOverRowIndex]+".unity");
			if (_scene!=null)
			{
				EditorGUIUtility.PingObject(_scene.GetInstanceID());	
			}
			*/

			try
			{
			EditorGUIUtility.PingObject(
							AssetDatabase.LoadMainAssetAtPath( 
						                                  (string)resultItems[mouseOverRowIndex]["path"]
													)
				);
			}catch{

			}
			
		}else{
			GUI.FocusControl("SearchField");
		}

		/*
		if (rssFeed!=null)
		{
			foreach (RssItem item in rssFeed.Items)
			{
				GUILayout.Label(item.Title);
			}
		}
		*/
	}



	private void OnGUI_ToolBar()
	{
		/*
		GUILayout.Space(6);
	//	EditorGUILayout.BeginHorizontal();
		GUILayout.BeginHorizontal("Search Field",GUILayout.ExpandWidth(true));
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();
		GUI.SetNextControlName ("SearchField");
		searchString = GUILayout.TextField(searchString,"Label");

		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();

		GUILayout.FlexibleSpace();

		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();

		if (wwwSearch!=null)
		{
			GUILayout.Label("Searching...","Label Row Plain");
		}else{
			if (GUILayout.Button("go","Button Small",GUILayout.Width(40),GUILayout.ExpandHeight(true)))
			{
				SearchRep();
			}
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
*/

		EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
			searchBox.OnGUI();

		GUILayout.Space(5);

		try{
			if (wwwSearch==null)
			{
				if (
					FsmEditorGUILayout.MiniButton(new GUIContent("Search"),GUILayout.Width(70) )  
					)                       
				{
					SearchRep();
				}
			}else{
				GUILayout.Label("Searching",GUILayout.Width(66));
			}
		}catch{
			//GUILayout.Label("Project assets being processed, please wait");
			return;
		}
		GUILayout.EndHorizontal();


		/*
		if (FsmEditorGUILayout.ToolbarSettingsButton())
		{
			//GenerateSettingsMenu().ShowAsContext();
		}
		*/
		//GUILayout.Space(-5);
		
	//	GUILayout.EndHorizontal();

	}


	private void OnGUI_BottomPanel()
	{

		if ( EditorApplication.isCompiling)
		{
			//FsmEditorGUILayout.Divider();
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal("Table Row Red Last",GUILayout.Width(position.width+3));

					GUILayout.Label("UNITY IS COMPILING","Label Row Red");
				GUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
	//	FsmEditorGUILayout.Divider();

		/*
		if (selectedAction != null)
		{
			// Action name and help button
			
			GUILayout.BeginHorizontal();
			
			GUILayout.Label("selected action description");
			
			GUILayout.EndHorizontal();
			
			EditorGUILayout.Space();
		//	FsmEditorGUILayout.Divider();

		}

		
		
		// Bottom toolbar
		
		GUILayout.BeginHorizontal();
		
		if (FsmEditor.SelectedState == null )  //|| selectedAction == null)
		{
			GUI.enabled = false;
		}
		
		if (GUILayout.Button(new GUIContent("Add Action To State")))
		{
			AddSelectedActionToState();
		}
		
		GUILayout.EndHorizontal();
		
		EditorGUILayout.Space();

		*/
	}

	private void AddSelectedActionToState()
	{
		if (FsmEditor.SelectedState == null)
		{
			return;
		}
		
		#if PREVIEW_VERSION
		Dialogs.PreviewVersion();
		#else
	//	FsmEditor.StateInspector.AddAction(selectedAction);
		//FinishAddAction();
		#endif
	}


	void OnGUI_ActionList()
	{

		if (resultCount==0)
		{
			//GUILayout.Label("No result");
			return;
		}else{


			//GUILayout.Label("result: "+resultCount);


			if (resultItems==null)
			{
				ParseSearchResult(rawSearchResult);
				Repaint ();
			}
		}

		if (resultItems==null)
		{
			return;
		}


		if (Event.current.type == EventType.Repaint)
			rowsArea = new Rect[resultItems.Length];


		if(downloads!=null)
		{
		//	GUILayout.Label("Downloads: "+downloads.Count);
		}

		Vector2 _scrollNew = GUILayout.BeginScrollView(_scroll);

		if (_scrollNew!=_scroll)
		{
			_scroll = _scrollNew;
			lastMousePosition = Vector2.zero;
			Repaint();
		}

		int i=0;

		foreach(Hashtable item in resultItems)
		{
			OnGUI_ActionItem(item,i);
			i++;
		}
		GUILayout.EndScrollView();
	}

	string mouseOverAction ="";



	void OnGUI_ActionItem(Hashtable item,int rowIndex)
	{
		// get the row style
		string rowStyle ="Middle";
		if (resultItems.Length==1)
		{
			rowStyle = "Alone";
		}else if (rowIndex==0) 
		{
			rowStyle = "First";
		}else if (rowIndex == (resultItems.Length-1) )
		{
			rowStyle = "Last";
		}

		// find details about the item itself
		string url = (string)item["RepositoryRawUrl"];
		bool downloading = !string.IsNullOrEmpty(url) && downloadsLUT.ContainsKey(url) ;

		string itemPath = (string)item["path"];
		string category = (string)item["category"];
		string unity_version = (string)item["unity_version"];
		string itemAssetsPath = itemPath.Substring(6);
		string assetPath = Application.dataPath+itemAssetsPath;
		bool fileExists = File.Exists(assetPath);

		// define the row style based on the item properties.
		string rowStyleType = "Plain";
	
		if (fileExists)
		{
			rowStyleType = "Green";
			/*
			if (scene.Equals(""))
			{
				rowStyleType = "Orange";
			}else{
				if (scenePaths[sceneIndex].Equals(""))
				{
					rowStyleType = "Red";
				}else{
					rowStyleType = "Green";
				}
			}
			*/
		}

		if (downloading)
		{
			rowStyleType = "Orange";
		}




		string _name = (string)item["name"];

		GUIContent bgContent = GUIContent.none;


		GUILayout.BeginVertical(bgContent,"Table Row "+rowStyleType+" "+rowStyle);
		GUILayout.BeginHorizontal();
		GUILayout.Label(FsmEditorUtility.NicifyVariableName(_name.Substring(0,_name.Length-3)),"Label Row "+rowStyleType,GUILayout.MinWidth(0));
			GUILayout.FlexibleSpace();



		if (mouseOverAction == _name)
		{
			var eventType = Event.current.type;
			
			if (eventType == EventType.MouseDown)
			{	

			//	string guid = AssetDatabase..AssetPathToGUID((string)item["path"]);
			//	Debug.Log(itemPath);
			//	EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(itemPath));

				SelectAction(_name);

				if (Event.current.clickCount > 1)
				{
					AddSelectedActionToState();
				}
				
				GUIUtility.ExitGUI();
				return;
			}
		}

		if (downloading)
		{
			GUILayout.Label("downloading...","Label Row "+rowStyleType,GUILayout.Width(80));
		}else if(mouseOverRowIndex==rowIndex )
		{
			 if (fileExists)
			{
				/*
				if (GUILayout.Button("Delete","Button Small",GUILayout.Width(50)))
				{
					DeleteItem(item);
					Repaint ();
					GUIUtility.ExitGUI();
					return;
				}
				*/
				GUILayout.Label("imported","Label Row "+rowStyleType,GUILayout.Width(50));
			}else{
				if (GUILayout.Button("Get","Button Small",GUILayout.Width(50)))
				{
					ImportItem(item);
					Repaint ();
					GUIUtility.ExitGUI();
					return;
				}
			}
		}
		GUILayout.EndHorizontal();

		// tags

			GUILayout.BeginHorizontal();

		GUILayout.Label("Unity "+unity_version,"Tag Small "+rowStyleType);
		GUILayout.Label(category,"Tag Small "+rowStyleType);
		GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		GUILayout.EndVertical();



		if(rowsArea!=null && rowIndex<rowsArea.Length)
		{
			rowsArea[rowIndex] = GUILayoutUtility.GetLastRect();
		}


	}

	
	private void SelectAction(string actionName)
	{
		if (actionName == selectedAction)
		{
			return;
		}

		selectedAction = actionName;
		
		Repaint();
	}

	
	void SearchRep()
	{
		/*
		if (rssFeed!=null)
		{
			foreach (RssItem item in rssFeed.Items)
			{
				Debug.Log(item.Title);
			}
		}
		*/

		string _filter = "";// "__ECO__";

		bool _test = false;
		if ( _test)
		{
			if (searchBox.SearchMode==1)
			{
				_filter += " __ACTION__";
			}else if (searchBox.SearchMode==2)
			{
				_filter += " __TEMPLATE__ ";
			}if (searchBox.SearchMode==3)
			{
				_filter += " __SAMPLE__ ";
			}
		}else{
			_filter += " __ACTION__";
		}


		WWWForm _form = new WWWForm(); // DO NOT WORK: I get 405 http error.

		_form.AddField("filter",_filter);
		string url = "http://www.fabrejean.net/projects/playmaker_ecosystem/search";

		url += "/"+WWW.EscapeURL(_filter +" "+ searchString);


		string mask = "U3";
		if ( Application.unityVersion.StartsWith("4."))
		{
			mask += "U4";
		}
		url += "?repository_mask="+mask;
//		Debug.Log(url);

		wwwSearch = new WWW(url);//,_form);
	}

	void OnInspectorUpdate() {

		if (wwwSearch!=null)
		{
			if (wwwSearch.isDone)
			{
				rawSearchResult = wwwSearch.text;
				wwwSearch.Dispose();
				wwwSearch = null;

				ParseSearchResult(rawSearchResult);

				Repaint();
			}
		}

		if (downloads!=null && downloads.Count>0)
		{
			for(int i =(downloads.Count-1);i>=0;i--)
			{
				//Debug.Log("Checking download "+i);
				WWW _www = downloads[i];
				if(_www.isDone){
					ProceedWithImport(_www.url,_www.text,_www.bytes);
					_www.Dispose();
					downloads.RemoveAt(i);
					Repaint();
				}
			}
		}

	}	


	void ParseSearchResult(string jsonString)
	{
		try {
			searchResultHash =  (Hashtable)JSON.JsonDecode(jsonString);
			if (searchResultHash == null)
			{
				Debug.LogWarning("json content is null");
				return;
			}

		} catch (System.Exception e) {
			Debug.LogWarning("Json parsing error "+e.Message);
			return;
		}

		resultCount = (int)searchResultHash["total_count"];
		//Debug.Log("count:"+resultCount);

		// reset LUT
		itemsLUT = null;

		ArrayList _arrayList = (ArrayList)searchResultHash["items"];
		resultItems = _arrayList.ToArray(typeof(Hashtable)) as Hashtable[];

	}

	void DeleteItem(Hashtable item)
	{

		AssetDatabase.DeleteAsset((string)item["path"]);
		AssetDatabase.Refresh();
	}

	void ImportItem(Hashtable item)
	{
		string itemPath = (string)item["path"];
		Hashtable rep = (Hashtable)item["repository"];
		string repositoryPath = (string)rep["full_name"];

		//string guid = AssetDatabase.AssetPathToGUID(itemPath);
//		Debug.Log(itemPath+" -> "+guid);


		//if (! string.IsNullOrEmpty(guid))
		//{
		//	Debug.Log(itemPath+" already exists");
		//}else{

			string itemAssetsPath = itemPath.Substring(6);

			string assetPath = Application.dataPath+itemAssetsPath;
//			Debug.Log(itemPath+" -> asset path -> "+assetPath);


			string itemPathEscaped = itemPath.Replace(" ","%20");

		//string url = "https://github.com/jeanfabre/"+RepositoryPath+"/blob/master/"+itemPathEscaped;
		//string url = "https://raw.github.com/"+RepositoryPath+"/master/"+itemPathEscaped;
		string url = __REST_URL_BASE__ +"download?repository="+ Uri.EscapeDataString(repositoryPath)+"&file="+ Uri.EscapeDataString(itemPathEscaped);

		item["RepositoryRawUrl"] = url;

		//Debug.Log("ImportItem "+url);

		DownloadRawContent(assetPath,url,item);
	//	}

	}

	void DownloadRawContent(string assetPath,string url,Hashtable item)
	{
	//	Debug.Log("DownloadRawContent "+assetPath+" "+url);
		if (downloadsLUT==null)
		{
			downloadsLUT = new Dictionary<string, string>();

		}

		if (downloads==null)
		{
			downloads = new List<WWW>();
		}

		if (itemsLUT==null)
		{
			itemsLUT = new Dictionary<string, Hashtable>();	
		}


		if (!downloadsLUT.ContainsKey(url))
		{
			downloads.Add(new WWW(url));
			downloadsLUT.Add(url,assetPath);
			itemsLUT.Remove(url);
			itemsLUT.Add(url,item);
		}

	}


	void ProceedWithImport(string url,string rawContent,byte[] rawBytes)
	{
		//Debug.Log("ProceedWithImport for "+url+" "+rawContent);

		string assetPath = downloadsLUT[url];
		downloadsLUT.Remove(url);

		Hashtable item =  itemsLUT[url];

		Hashtable rep = (Hashtable)item["repository"];
		string repositoryPath = (string)rep["full_name"];


		// is it a template?
		if (rawContent.Contains("__TEMPLATE__"))
		{
		//	Debug.Log("This is a template actually ");

			Hashtable _templateMeta = (Hashtable)JSON.JsonDecode(rawContent);

			if (_templateMeta==null)
			{
				Debug.LogWarning("Could not get the json of this template ");
				return;
			}
		//	Debug.Log(_templateMeta.ContainsKey("unitypackage"));
			object _tmpPath = _templateMeta["unitypackage"];
			string _templatePath = _tmpPath.ToString();

			//string url = "https://github.com/jeanfabre/"+RepositoryPath+"/blob/master/"+itemPathEscaped;

			//string _templateUrl = "https://raw.github.com/"+RepositoryPath+"/master/"+_templatePath.Replace(" ","%20");

			string _templateUrl = __REST_URL_BASE__ +"download?repository="+ Uri.EscapeDataString(repositoryPath)+"&file="+ Uri.EscapeDataString(_templatePath);


			DownloadRawContent(_templatePath,_templateUrl,item);

			/*
			ArrayList _templateDependancies = (ArrayList)_templateMeta["script dependancies"];
		
			foreach(object dScript in _templateDependancies)
			{
				string _dscript = (string)dScript;
				Debug.Log(_dscript);
				
				string itemPathEscaped = _dscript.Replace(" ","%20");
				
				string dscripturl = "https://raw.github.com/"+RepositoryPath+"/master/"+itemPathEscaped;
				DownloadRawContent(_dscript,dscripturl);
				
			}
			*/

			return;
		}


	
		// check for Meta data
		// .*?
		Match match = Regex.Match(rawContent,@"(?<=EcoMetaStart)[^>]*(?=EcoMetaEnd)",RegexOptions.IgnoreCase);
		
		// Here we check the Match instance.
		if (match.Success)
		{
		//	Debug.Log("we have meta data :" + match.Value);

			Hashtable _meta = (Hashtable)JSON.JsonDecode(match.Value);
			ArrayList _dependancies = (ArrayList)_meta["script dependancies"];
			if (_dependancies!=null)
			{
				foreach(object dScript in _dependancies)
				{
					string _dscript = (string)dScript;
					//Debug.Log(_dscript);

				//	string itemPathEscaped = _dscript.Replace(" ","%20");
					
					//string dscripturl = "https://raw.github.com/"+RepositoryPath+"/master/"+itemPathEscaped;
					string dscripturl = __REST_URL_BASE__ +"download?repository="+ Uri.EscapeDataString(repositoryPath)+"&file="+ Uri.EscapeDataString(_dscript);

					//Debug.Log(dscripturl);
					DownloadRawContent(_dscript,dscripturl,item);

				}
			}
		}

		if (url.Contains(".unitypackage"))
		{
	//		Debug.Log("we have a unitypackage!!"+assetPath);

			string unityPackageTempFile = Application.dataPath.Substring(0,Application.dataPath.Length-6) +"Temp/PlayMakerEcosystem.downloaded.unityPackage";

			FileInfo _tempfileInfo = new FileInfo(Application.dataPath);
			if (!Directory.Exists(_tempfileInfo.DirectoryName))
			{
				Directory.CreateDirectory(_tempfileInfo.DirectoryName);
			}
			
			//	if (string.IsNullOrEmpty)
			File.WriteAllBytes(unityPackageTempFile,rawBytes);

			AssetDatabase.ImportPackage(unityPackageTempFile,true);
			return;
		}


		FileInfo _fileInfo = new FileInfo(assetPath);
		if (!Directory.Exists(_fileInfo.DirectoryName))
		{
			Directory.CreateDirectory(_fileInfo.DirectoryName);
		}

		File.WriteAllBytes(assetPath,rawBytes);

		AssetDatabase.Refresh();

	}

	/*
	string BuildSearchUrl(string searchQuery)
	{
		//"https://api.github.com/search/code?q=SimpleExample+in:language:cs+repo:pbhogan/InControl";
		string url = searchUrlBase;

		Debug.Log(searchBox.SearchMode);

		string _filter = "";// "__ECO__";

		if (searchBox.SearchMode==1)
		{
			_filter += " __ACTION__";
		}else if (searchBox.SearchMode==2)
		{
			_filter += " __TEMPLATE__ ";
		}if (searchBox.SearchMode==3)
		{
			_filter += " __SAMPLE__ ";
		}



		url += "code?q="+WWW.EscapeURL(_filter+" "+searchQuery);
		//url += "+extension:txt";
		url += "+repo:"+RepositoryPath;

		Debug.Log("search url = "+url);
		return url;

	}
*/



	void AddGlobalVariable(string name,VariableType type)
	{
		FsmVariable.AddVariable(FsmVariables.GlobalVariables, type, name);
		FsmEditor.SaveGlobals();

	}


}