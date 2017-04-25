using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Vuforia;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;

public class SimpleCloudHandler : MonoBehaviour, ICloudRecoEventHandler {
	private CloudRecoBehaviour mCloudRecoBehaviour;
	private ObjectTracker mImageTracker;
	private bool mIsScanning = false;
	private GameObject target_model;
	private GameObject character_model;
	private GameObject bubbleText_model;
	private GameObject button_model;
	private GameObject newImageTarget;
	ImageTargetAbstractBehaviour imageTargetBehaviour;
	private TextMesh bubble_text;
	private string popularItems;
	private string token = "BcvGaMJ2kaPk6VgHEEfnNBBzqhyLZGMz8YB-BQVk224gYiXkky1xPWRWjDx06BLuCdR12arUq-Aa5RkBKfoazDANJ31WymPAqS22gyyXajFSrtZJ_x_DaygNfZXuWHYx";

	//image target behavior
	public ImageTargetBehaviour ImageTargetTemplate;
	public GameObject character_main;

	//styles, textures, and skins
	public GUISkin skin;
	public Texture texture_button_start;
	public GUIStyle style_title;
	public GUIStyle style_button;
	public GUIStyle style_reviews;
	public GUIStyle style_button_main;
	public GUIStyle style_AR_title;
	public GUIStyle style_button_next;
	public GUIStyle style_button_previous;
	public GUIStyle style_description_box;
	public GUIStyle style_landmark_description;
	public GUIStyle style_item_title;
	public GUIStyle style_button_AR_main;
	public GUIStyle style_button_showModel;
	public GUIStyle style_businessHours;
	public GUIStyle style_button_AR_return;
	public GUIStyle style_background_main;
	public GUIStyle style_item_price;
	public GUIStyle style_line_divider;
	public GUIStyle style_target_priceyness;
	public GUIStyle style_icon_hours;
	public GUIStyle style_icon_yelp;
	public GUIStyle style_icon_phone;
	public GUIStyle style_star_rating;
	public GUIStyle style_scanningLine;
	public GUIStyle style_AR_subtitle;
	public GUIStyle style_reviews_moreButton;
	private string priceyness_sign;
	private string priceyness_empty;

	//UI variables
	private bool showMainPage = true;
	private bool showPopularDishes = false;
	private bool showHours = false;
	private bool showScanningPage = false;
	private bool showTargetDescription = false;
	private bool showTargetPopularItems = false;
	private bool showTargetBusinessHours = false;
	private bool showTargetReviews = false;
	private bool background_color = false;
	private bool background_home = false;
	private bool showTargetModel = false;


	private string mTargetMetadata = "";
	public int i = 0;
	public MyClass Targetdata;
	public Target_information target_info;
	//public ScanLine scanLine;
	//public SimpleCharacterControl animation;
	//load images:variable
	private Object[] textures;
	private List<string> reviews_text;
	private List<string> reviews_url;
	private List<string> reviews_rating;
	private List<string> target_hours;
	public List<int> image_identifier;
	public List<string> description_identifier;
	public List<string> title_identifier;
	public List<string> price_identifier;
	private float y = 0;
	private float mTime = 0;
	private float mScanDuration = 9/5;//seconds
	private bool mMovingDown = true;
	private float u = 0;
	private GameObject go;
	//class that will hold the image target metadata
	[System.Serializable]
	public class MyClass
  	{
		//what all image-targets' metadata share: a type (monument/restaurant/bar/cafe)
		//and description(the target's main description/title: e.g. "Starbucks, a famous coffee shop")
	    public string Type;
		public string Name;
		public string id = "";
	    public string Description;

		//items:
		public string Item_1 = "";
		public string Item_1_description = "";
		public string Item_1_price = "";
		public string Item_2 = "";
		public string Item_2_description = "";
		public string Item_2_price = "";
		public string Item_3 = "";
		public string Item_3_description = "";
		public string Item_3_price = "";

  	}
	//class that will hold the target information from yelp's API request
	[System.Serializable]
	public class Target_information
	{
		public string url;
		public string price;
		public string rating;
		public string phone;
		public bool is_closed;
	}

	// Use this for initialization
	void Start () {
//		StartCoroutine(GetTargetReviews());
//		StartCoroutine (GetTargetInformation ());
		// register this event handler at the cloud reco behaviour
		mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();

		if (mCloudRecoBehaviour)
		{
			mCloudRecoBehaviour.RegisterEventHandler(this);
		}

		go = GameObject.CreatePrimitive(PrimitiveType.Plane);
		textures = Resources.LoadAll("textures");

	}
	public void OnInitialized()
	{
		Debug.Log ("Cloud Reco initialized");
		mImageTracker = (ObjectTracker)TrackerManager.Instance.GetTracker<ObjectTracker>();
	}

	public void OnInitError(TargetFinder.InitState initError)
	{
		Debug.Log ("Cloud Reco init error " + initError.ToString());
	}

	public void OnUpdateError(TargetFinder.UpdateState updateError)
	{
		Debug.Log ("Cloud Reco update error " + updateError.ToString());
	}

	public void OnStateChanged(bool scanning)
	{
		mIsScanning = scanning;

		if (scanning)
		{
			ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
			tracker.TargetFinder.ClearTrackables(false);
		}
		//ShowScanLine(mIsScanning);
	}

	// Here we handle a cloud target recognition event
	public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult) {
		// save metadata in string
		mTargetMetadata = targetSearchResult.MetaData;

		//turn JSON into object
		Targetdata = JsonUtility.FromJson<MyClass>(mTargetMetadata);

		//set text in bubble dialogue
		bubble_text = GameObject.Find("BubbleMessage").GetComponent<TextMesh> ();
		bubble_text.text = "Hurray! you found\na "+Targetdata.Type+"!";
//		bubble_text.fontSize = 10;
//		bubble_text.anchor = TextAnchor.UpperLeft;
//		bubble_text.alignment = TextAlignment.Center;

		// duplicate the referenced image target
		newImageTarget = Instantiate(ImageTargetTemplate.gameObject) as GameObject;
		GameObject augmentation = null;
		if( augmentation != null )
			augmentation.transform.parent = newImageTarget.transform;

		// enable the new result with the same ImageTargetBehaviour:
		imageTargetBehaviour = mImageTracker.TargetFinder.EnableTracking(targetSearchResult, newImageTarget);

		switch( Targetdata.Type ){

		case "Clothing store" :

			target_model = imageTargetBehaviour.gameObject.transform.Find("Clothing Store").gameObject;
			Destroy( imageTargetBehaviour.gameObject.transform.Find("Landmark").gameObject );
			Destroy( imageTargetBehaviour.gameObject.transform.Find("Restaurant").gameObject );
			Destroy( imageTargetBehaviour.gameObject.transform.Find("Cafe").gameObject );
			Destroy( imageTargetBehaviour.gameObject.transform.Find("Bar").gameObject );

			break;

		case "Landmark" :

			target_model = imageTargetBehaviour.gameObject.transform.Find("Landmark").gameObject;
			Destroy( imageTargetBehaviour.gameObject.transform.Find("Clothing Store").gameObject );
			Destroy( imageTargetBehaviour.gameObject.transform.Find("Restaurant").gameObject );
			Destroy( imageTargetBehaviour.gameObject.transform.Find("Cafe").gameObject );
			Destroy( imageTargetBehaviour.gameObject.transform.Find("Bar").gameObject );

			break;

		case "Restaurant" :

			target_model = imageTargetBehaviour.gameObject.transform.Find("Restaurant").gameObject;
			Destroy( imageTargetBehaviour.gameObject.transform.Find("Landmark").gameObject );
			Destroy( imageTargetBehaviour.gameObject.transform.Find("Clothing Store").gameObject );
			Destroy( imageTargetBehaviour.gameObject.transform.Find("Cafe").gameObject );
			Destroy( imageTargetBehaviour.gameObject.transform.Find("Bar").gameObject );

			break;

		case "Cafe" :

			target_model = imageTargetBehaviour.gameObject.transform.Find("Cafe").gameObject;
			Destroy( imageTargetBehaviour.gameObject.transform.Find("Landmark").gameObject );
			Destroy( imageTargetBehaviour.gameObject.transform.Find("Restaurant").gameObject );
			Destroy( imageTargetBehaviour.gameObject.transform.Find("Clothing Store").gameObject );
			Destroy( imageTargetBehaviour.gameObject.transform.Find("Bar").gameObject );

			break;

		case "Bar" :

			target_model = imageTargetBehaviour.gameObject.transform.Find("Bar").gameObject;
			Destroy( imageTargetBehaviour.gameObject.transform.Find("Landmark").gameObject );
			Destroy( imageTargetBehaviour.gameObject.transform.Find("Restaurant").gameObject );
			Destroy( imageTargetBehaviour.gameObject.transform.Find("Clothing Store").gameObject );
			Destroy( imageTargetBehaviour.gameObject.transform.Find("Cafe").gameObject );

			break;

		}

		//character model
		character_model = imageTargetBehaviour.gameObject.transform.Find("Character").gameObject;
		//bubble text model:
		bubbleText_model = imageTargetBehaviour.gameObject.transform.Find("BubbleText").gameObject;
		//button model
		button_model = imageTargetBehaviour.gameObject.transform.Find("Button").gameObject;

		mCloudRecoBehaviour.CloudRecoEnabled = false;
		//ShowScanLine(false);
		mIsScanning = false;

		//if target is restaurant/cafe/bar do a yelp public API search for more information
		if (Targetdata.Type != "Landmark") {
			StartCoroutine (GetTargetReviews ());
			StartCoroutine (GetTargetInformation ());
		}
	}

	//get target reviews from Yelp's public API
	IEnumerator GetTargetReviews()
	{
		//Targetdata.id = "shalimar-restaurant-ann-arbor-2";
		using (UnityWebRequest www = UnityWebRequest.Get("https://api.yelp.com/v3/businesses/"+Targetdata.id+"/reviews" ))
		{
			www.SetRequestHeader("Authorization", "Bearer "+token);
			yield return www.Send();

			if (www.isError){
				Debug.Log(www.error);
			}else
			{
				JSONNode node=JSON.Parse(www.downloadHandler.text);

				reviews_text=new List<string>();
				reviews_rating=new List<string>();
				reviews_url=new List<string>();
				for (int i = 0; i < node ["reviews"].Count; i++) {
					reviews_text.Add(node["reviews"][i]["text"].Value);
					reviews_rating.Add(node["reviews"][i]["rating"].Value);
					reviews_url.Add(node["reviews"][i]["url"].Value);
				}
				// Show results as text
				Debug.Log(reviews_text.Count);
				Debug.Log(reviews_rating.Count);
			}
		}
	}

	//get Target information from Yelp's public API
	IEnumerator GetTargetInformation()
	{
		using (UnityWebRequest www = UnityWebRequest.Get("https://api.yelp.com/v3/businesses/"+Targetdata.id))
		{
			www.SetRequestHeader("Authorization", "Bearer "+token);
			yield return www.Send();

			if (www.isError){
				Debug.Log(www.error);
			}else
			{
				target_info = JsonUtility.FromJson<Target_information>(www.downloadHandler.text);
				JSONNode node=JSON.Parse(www.downloadHandler.text);
				target_hours=new List<string>();
				for (int i = 0; i < node ["hours"][0]["open"].Count; i++) {
					target_hours.Add(node["hours"][0]["open"][i]["start"].Value.Insert(2,":"));
					target_hours.Add(node["hours"][0]["open"][i]["end"].Value.Insert(2,":"));
				}

			}
		}
	}

	void OnGUI() {

		//background color: on all except for home page and scanning page
		if (background_color) {
			GUI.Box (new Rect (0, 0, Screen.width, Screen.height), "", style_background_main);
		}
		//background color: home page
		if (background_home) {
			GUI.Box (new Rect (0, 0, Screen.width, Screen.height/2), "", style_background_main);
			GUI.Box (new Rect (0, Screen.height/2, 2*Screen.width/5, Screen.height/2), "", style_background_main);
			GUI.Box (new Rect (3*Screen.width/5, Screen.height/2, 2*Screen.width/5, Screen.height/2), "", style_background_main);
			GUI.Box (new Rect (2*Screen.width/5, 4*Screen.height/5-20, Screen.width/5, Screen.height/5+20), "", style_background_main);
		}

		//title: on all pages
		GUI.Label(new Rect(Screen.width/2-160,20,320,188), "", style_title);

		//line divider: all pages
		GUI.Label(new Rect(20,Screen.height-100,Screen.width-40,67), "", style_line_divider);


		//home page
		if (showMainPage) {

			//background
			background_color = false;
			background_home = true;

			//show character animation
			character_main.SetActive (true);

			//show APP description
			GUI.TextArea (new Rect (Screen.width / 2-400, 250, 800 , 670), "This App lets users scan buildings and landmarks with their phone’s camera in Ann Arbor, Michigan, and with its Augmented Reality technology, returns and overlays target information. It uses Cloud Recognition to recognize many buildings such as restaurants, bars, cafes, clothing stores and landmarks. Simply hit the start button, then point your phone's camera at a target and information will be returned and augmented. Happy Scanning!", style_description_box);

			//button: start scanning
			if (GUI.Button (new Rect (Screen.width/2-350, 1550, 700, 200), "Start Scanning",style_button_main)) {
				showMainPage = false;
				showScanningPage = true;
				mCloudRecoBehaviour.CloudRecoEnabled = true;
				character_main.SetActive (false);
				background_home = false;
			}

		}

		//scanning page
		if (showScanningPage) {

			//background color off
			background_color = false;

			//animation
			GUI.TextField(new Rect (0, y, Screen.width, 50), "", style_scanningLine);

			//once found, show result on new page
			if(!mIsScanning) {
				showTargetModel = true;
				showScanningPage = false;
			}

		}

		//after scanning is complete: show augmented model
		if (showTargetModel){

			//if "More details" button is pressed
			if (!button_model.GetComponent<click>().condition) {

				//show next page
				showTargetDescription = true;
				showTargetModel = false;

				//destroy models
//				Destroy(newImageTarget);
//				Destroy (imageTargetBehaviour);
				Destroy (button_model);
				Destroy (target_model);
				Destroy (character_model);
				Destroy (bubbleText_model);
			}
		}


		//image target description
		if (showTargetDescription) {

			//background color
			background_color = true;

			//name of target
			GUI.Label (new Rect (Screen.width / 2, 300, 0, 0), Targetdata.Name, style_AR_title);
			GUI.Label (new Rect (Screen.width / 2, 360, 0, 0), Targetdata.Type, style_AR_subtitle);

			//button: return to main page
			if (GUI.Button (new Rect (50, 50, 90, 90), "", style_button_AR_return)) {
				showTargetDescription = false;
				showMainPage = true;
			}

			//other UI elements: will depend on object type

			//for restaurants/cafes/bars/shops:
			//there will be a "show popular items" button -> each button will be written differently e.g. "popular dishes"/"popular drinks"
			if (Targetdata.Type == "Cafe" || Targetdata.Type == "Bar") {
				popularItems = "Popular Items";
			} else if (Targetdata.Type == "Restaurant") {
				popularItems = "Popular Dishes";
			} else if (Targetdata.Type == "Clothing store") {
				popularItems = "Top Sellers";
			}

			//UI for restaurant/cafe/bar/shops are the same
			if (Targetdata.Type != "Landmark") {

				//description
				GUI.TextArea (new Rect (Screen.width / 2-400, 540, 800 , 400), Targetdata.Description, style_description_box);

				//priceyness level from 0-4. The 0-4 level will be written in the number of dollar signs: "$"
				//example: one red "$" and three white "$" will mean a priceyness level of 1 out of 4
				//Yelp's API gives us the priceyness level in "$" signs. So for a level of 2/4 it will give us "$$"
				//therefore we have to add the rest of the "white"(empty) $ signs
				//we do this by finding the difference between the total(4) and the number of $ signs returned from yelp
				priceyness_sign = target_info.price;
				int priceyness_difference =  4 - priceyness_sign.Length;
				priceyness_empty = "";
				for (var i = 0; i < priceyness_difference; i++) {
					priceyness_empty = priceyness_empty + "$";
				}

				//show priceyness level
				GUI.Label (new Rect (Screen.width/2, 1000, 0, 0), "<color=green>"+ priceyness_sign + "</color>" + "<color=grey>" + priceyness_empty + "</color>", style_target_priceyness);

				//show opening hours:
				GUI.Box(new Rect (285-150, 1150, 80, 80),"", style_icon_hours); //icon
				//style_icon_hours.richText = true;
				string text_open;
				if (target_info.is_closed) {
					text_open = "<color=green>"+"Open now"+"</color>";
				} else{
					text_open = "<color=red>" + "Closed now" + "</color>";
				}
				GUI.Label(new Rect (285+40, 1168, 0, 0),text_open, style_icon_hours);  //text

				//show phone number
				GUI.Box(new Rect (797-180, 1150, 80, 80),"", style_icon_phone); //icon
				GUI.Label(new Rect (797+20, 1170, 0, 0), target_info.phone, style_icon_phone);  //text

				//show rating: will be done the same way as the priceyness rating
				int rating_star_count = (int)Mathf.Round(float.Parse(target_info.rating));
				int rating_difference =  5 - rating_star_count;
				string rating_star = "";
				string rating_star_empty = "";
				for (var i = 0; i < rating_star_count; i++) {
					rating_star = rating_star + "Z ";
				}
				for (var i = 0; i < rating_difference; i++) {
					rating_star_empty = rating_star_empty + "Z";
				}
				GUI.Label (new Rect (Screen.width/2, 430, 0, 0), "<color=#e78200>"+ rating_star + "</color>" + "<color=grey>" + rating_star_empty + "</color>", style_star_rating);


				//button: popular items
				if (GUI.Button (new Rect (93, 1300, 400, 190), popularItems, style_button)) {
					showTargetPopularItems = true;
					showTargetDescription = false;
				}

				//button: business hours
				if (GUI.Button (new Rect (93, 1550, 400, 190), "Business Hours", style_button)) {
					showTargetBusinessHours = true;
					showTargetDescription = false;
				}

				//button: reviews
				if (GUI.Button (new Rect (1080-93-400, 1300, 400, 190), "Reviews", style_button)) {
					showTargetReviews = true;
					showTargetDescription = false;
				}

				//button: yelp icon link
				if (GUI.Button (new Rect (1080-93-400, 1550, 400, 190), "Yelp", style_button)) {
					Application.OpenURL(target_info.url);
				}
			}

			//UI for Landmark
			else{


				//get item descriptions
				description_identifier = new List<string>();
				description_identifier.Add(Targetdata.Item_1_description);
				description_identifier.Add(Targetdata.Item_2_description);
				description_identifier.Add(Targetdata.Item_3_description);

				//image
				int x = FindImageItem (Targetdata.Name);
				Texture2D image = (Texture2D)textures[x];
				go.GetComponent<Renderer>().material.mainTexture = image;
				GUI.DrawTexture (new Rect (Screen.width / 2 - 400, 350, 800, 600), image);

				//button next item
				if (GUI.Button (new Rect (Screen.width - 110, 1260, 100, 200),"", style_button_next)) {
					if (i == description_identifier.Count - 1) {
						i = 0;
					} else {
						i++;
					}
				}

				//button previous item
				if (GUI.Button (new Rect (10, 1260, 100, 200), "", style_button_previous)) {
					if (i == 0) {
						i = description_identifier.Count-1;
					} else {
						i--;
					}
				}

				//description of Landmark:
				GUI.TextArea (new Rect (Screen.width/2 - 400, 1000, 800, 720), description_identifier[i], style_landmark_description);
			}

		}


		//show popular items
		if (showTargetPopularItems) {

			//get item images
			image_identifier = new List<int>();
			image_identifier.Add(FindImageItem(Targetdata.Item_1));
			image_identifier.Add(FindImageItem(Targetdata.Item_2));
			image_identifier.Add(FindImageItem(Targetdata.Item_3));
			//get item descriptions
			description_identifier = new List<string>();
			description_identifier.Add(Targetdata.Item_1_description);
			description_identifier.Add(Targetdata.Item_2_description);
			description_identifier.Add(Targetdata.Item_3_description);
			//get item names
			title_identifier = new List<string>();
			title_identifier.Add(Targetdata.Item_1);
			title_identifier.Add(Targetdata.Item_2);
			title_identifier.Add(Targetdata.Item_3);
			//get item prices
			price_identifier = new List<string>();
			price_identifier.Add(Targetdata.Item_1_price);
			price_identifier.Add(Targetdata.Item_2_price);
			price_identifier.Add(Targetdata.Item_3_price);

			//title: Popular Items
			GUI.Label (new Rect (Screen.width / 2, 300, 0, 0), popularItems, style_AR_title);
			//show item name
			GUI.Label (new Rect (Screen.width/2, 400, 0, 0), title_identifier [i] , style_item_title);
			//show item description
			GUI.TextArea (new Rect (Screen.width/2 - 400, 1250, 800, 350), description_identifier [i], style_description_box);
			//show item price
			GUI.Label (new Rect (Screen.width/2, 1670, 0, 0), price_identifier [i] , style_item_price);
			//show image
			Texture2D image = (Texture2D)textures[image_identifier[i]];
			go.GetComponent<Renderer>().material.mainTexture = image;
			GUI.DrawTexture (new Rect (Screen.width / 2 - 400, 500, 800, 650), image);

			//button next item
			if (GUI.Button (new Rect (Screen.width - 110, 725, 100, 200),"", style_button_next)) {
				if (i == image_identifier.Count - 1) {
					i = 0;
				} else {
					i++;
				}
			}

			//button previous item
			if (GUI.Button (new Rect (10, 725, 100, 200), "", style_button_previous)) {
				if (i == 0) {
					i = image_identifier.Count-1;
				} else {
					i--;
				}
			}

			//button: return
			if (GUI.Button (new Rect (50, 50, 90, 90), "", style_button_AR_return)) {
				showTargetPopularItems = false;
				showTargetDescription = true;
			}

		}


		//show business hours:
		if (showTargetBusinessHours) {
			// title
			GUI.Label (new Rect (Screen.width/2, 350, 0, 0), "Business Hours", style_AR_title);

			//show business hours
			string hours_text =
				"Monday: " + target_hours[0] + " - "  + target_hours[1]  +  "\n\n" +
				"Tuesday: " + target_hours[2] + " - "  + target_hours[3]  +  "\n\n" +
				"Wednesday: " + target_hours[4] + " - "  + target_hours[5]  +  "\n\n" +
				"Thursday: " + target_hours[6] + " - "  + target_hours[7]  +  "\n\n" +
				"Friday: " + target_hours[8] + " - "  + target_hours[9]  +  "\n\n" +
				"Saturaday: " + target_hours[10] + " - "  + target_hours[11]  +  "\n\n" +
				"Sunday: " + target_hours[12] + " - "  + target_hours[13];
			GUI.TextArea (new Rect (Screen.width/2 - 340, 520, 680, 1000), hours_text, style_businessHours);
			//JSONdata.hours

			//button: return to main page
			if (GUI.Button (new Rect (50, 50, 90, 90), "", style_button_AR_return)) {
				showTargetBusinessHours = false;
				showTargetDescription = true;
			}

		}

		//show 3 reviews from yelp's public API:
		if (showTargetReviews) {

			// title
			GUI.Label (new Rect (Screen.width/2, 300, 0, 0), "Reviews", style_AR_title);

			//show reviews
			//show text of each review and rating in star signs
			List<string> ratings = new List<string>();
			List<string> ratings_empty = new List<string>();
			for (var i = 0; i < 3; i++) {
				int rating_star_count = (int)Mathf.Round(float.Parse(reviews_rating[i]));
				int rating_difference =  5 - rating_star_count;
				ratings.Add ("");
				ratings_empty.Add ("");
				for (var j = 0; j < rating_star_count; j++) {
					ratings [i] = ratings [i] + "Z";
				}
				for (var k = 0; k < rating_difference; k++) {
					ratings_empty [i] = ratings_empty [i] + "Z";
				}
			}
			GUI.Label (new Rect (Screen.width/2, 400, 0, 0), "<color=#e78200>"+ ratings[0] + "</color>" + "<color=grey>" + ratings_empty[0] + "</color>", style_star_rating);
			GUI.Label (new Rect (Screen.width/2, 880, 0, 0), "<color=#e78200>"+ ratings[1] + "</color>" + "<color=grey>" + ratings_empty[1] + "</color>", style_star_rating);
			GUI.Label (new Rect (Screen.width/2, 1360, 0, 0), "<color=#e78200>"+ ratings[2] + "</color>" + "<color=grey>" + ratings_empty[2] + "</color>", style_star_rating);
			GUI.TextArea (new Rect (200, 470, 680, 300), reviews_text[0], style_reviews);
			GUI.TextArea (new Rect (200, 950, 680, 300), reviews_text[1], style_reviews);
			GUI.TextArea (new Rect (200, 1430, 680, 300), reviews_text[2], style_reviews);

			//buttons -> read more: directs you to Yelp
			if (GUI.Button (new Rect (900, 570, 100, 100), "", style_reviews_moreButton)) {
				Application.OpenURL(reviews_url[0]);
			}
			if (GUI.Button (new Rect (900, 1050, 100, 100), "", style_reviews_moreButton)) {
				Application.OpenURL(reviews_url[1]);
			}
			if (GUI.Button (new Rect (900, 1530, 100, 100), "", style_reviews_moreButton)) {
				Application.OpenURL(reviews_url[2]);
			}


			//button: return
			if (GUI.Button (new Rect (50, 50, 90, 90), "", style_button_AR_return)) {
				showTargetReviews = false;
				showTargetDescription = true;
				Debug.Log ("hi");
			}
		}



	}










		// Display current 'scanning' status
		//GUI.Box (new Rect(Screen.width/2,Screen.height/2,Screen.width/4,Screen.height/4), mIsScanning ? "Scanning" : "Not scanning");
		// Display metadata of latest detected cloud-target
//		if (showAge) {
//			GUI.Box (new Rect (100, 200, 200, 50), "Metadata: " + JSONdata.Age);
//		}
		// If not scanning, show button
		// so that user can restart cloud scanning
		//if (!mIsScanning) {
//			if (GUI.Button(new Rect(100,300,600,200), "Restart Scanning")) {
//				// Restart TargetFinder
//				mCloudRecoBehaviour.CloudRecoEnabled = true;
//			}
		//}
		//if (GUI.Button (new Rect (100, 300, 200, 50), btnTexture)) {
			//showAge = !showAge;
		//}
		 //pointB = Event.current.mousePosition;
//		Vector2 pointA = new Vector2 (Screen.width/2, Screen.height/2);
//		Vector2 pointB = new Vector2 (Screen.width/3, Screen.height/3);
//		Drawing.DrawLine(pointA, pointB, Color.red, 1);


	//}

	void Update(){
		//scanning line
		u = mTime / mScanDuration;
		mTime += Time.deltaTime;
		if (u > 1)
		{
			// invert direction
			mMovingDown = !mMovingDown;
			u = 0;
			mTime = 0;
		}
		if (mMovingDown) {
			y = u * Screen.height;
		} else {
			y = Screen.height - u*Screen.height;
		}

		//rotate gameobject
		if (target_model != null) {
			if (target_model.activeSelf) {
				target_model.transform.Rotate (0, 1, 0);
			}
		}
	}

//	private void ShowScanLine(bool show)
//	{
//		// Toggle scanline rendering
//		if (scanLine != null)
//		{
//			Renderer scanLineRenderer = scanLine.GetComponent<Renderer>();
//			if (show)
//			{
//				// Enable scan line rendering
//				if (!scanLineRenderer.enabled)
//					scanLineRenderer.enabled = true;
//
//				scanLine.ResetAnimation();
//			}
//			else
//			{
//				// Disable scanline rendering
//				if (scanLineRenderer.enabled)
//					scanLineRenderer.enabled = false;
//			}
//		}
//	}
	int FindImageItem(string name) {
		for (int i=0;i<textures.Length;i++){
			if(textures[i].name == name){
				return i;
			}
			Debug.Log (i);
		}
		return 1;
	}
}
//	int Find






//	public class Drawing {
//
//		public static Texture2D lineTex = new Texture2D(1, 1); //Single pixel texture
//
//		public static void DrawLine(Rect rect) { DrawLine(rect, GUI.contentColor, 1.0f); }
//		public static void DrawLine(Rect rect, Color color) { DrawLine(rect, color, 1.0f); }
//		public static void DrawLine(Rect rect, float width) { DrawLine(rect, GUI.contentColor, width); }
//		public static void DrawLine(Rect rect, Color color, float width) { DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height), color, width); }
//		public static void DrawLine(Vector2 pointA, Vector2 pointB) { DrawLine(pointA, pointB, GUI.contentColor, 1.0f); }
//		public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color) { DrawLine(pointA, pointB, color, 1.0f); }
//		public static void DrawLine(Vector2 pointA, Vector2 pointB, float width) { DrawLine(pointA, pointB, GUI.contentColor, width); }
//		public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width) {
//
//			Matrix4x4 matrix = GUI.matrix; // Save current GUI matrix.
//			if ((pointB - pointA).magnitude < 0.001f) // Deny if Line too small
//				return;
//
//			Color savedColor = GUI.color;// Save current GUI color,
//			GUI.color = color;// and set the GUI color to the color parameter
//			pointB.y += width/2; //fixes missalignment from applying wrong angle
//
//			float angle = Vector3.Angle(pointB - pointA, Vector2.right);// Determine the angle of the line.
//
//			// Vector3.Angle always returns a positive number.
//			// If pointB is above pointA, then angle needs to be negative.
//			if (pointA.y > pointB.y) { angle = -angle; }
//
//
//			Vector3 pivot = new Vector2(pointA.x, pointA.y + width / 2); //Pivot is on pointA and shifted to the middle with + 0.5f
//			GUIUtility.RotateAroundPivot(angle, pivot);// Set the rotation for the line with pointA as the origin
//			GUI.DrawTexture(new Rect(pointA.x, pointA.y, (pointB - pointA).magnitude, width), lineTex); //Draws and scales the line
//
//			GUI.matrix = matrix; // Restore the GUI matrix...
//			GUI.color = savedColor; // ...and GUI color to previous values
//		}
//	}





























//	public static class Drawing
//	{
//		private static Texture2D aaLineTex = null;
//		private static Texture2D lineTex = null;
//		private static Material blitMaterial = null;
//		private static Material blendMaterial = null;
//		private static Rect lineRect = new Rect(0, 0, 1, 1);
//
//		// Draw a line in screen space, suitable for use from OnGUI calls from either
//		// MonoBehaviour or EditorWindow. Note that this should only be called during repaint
//		// events, when (Event.current.type == EventType.Repaint).
//		//
//		// Works by computing a matrix that transforms a unit square -- Rect(0,0,1,1) -- into
//		// a scaled, rotated, and offset rectangle that corresponds to the line and its width.
//		// A DrawTexture call used to draw a line texture into the transformed rectangle.
//		//
//		// More specifically:
//		//      scale x by line length, y by line width
//		//      rotate around z by the angle of the line
//		//      offset by the position of the upper left corner of the target rectangle
//		//
//		// By working out the matrices and applying some trigonometry, the matrix calculation comes
//		// out pretty simple. See https://app.box.com/s/xi08ow8o8ujymazg100j for a picture of my
//		// notebook with the calculations.
//		public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width, bool antiAlias)
//		{
//			// Normally the static initializer does this, but to handle texture reinitialization
//			// after editor play mode stops we need this check in the Editor.
//			#if UNITY_EDITOR
//			if (!lineTex)
//			{
//				Initialize();
//			}
//			#endif
//
//			// Note that theta = atan2(dy, dx) is the angle we want to rotate by, but instead
//			// of calculating the angle we just use the sine (dy/len) and cosine (dx/len).
//			float dx = pointB.x - pointA.x;
//			float dy = pointB.y - pointA.y;
//			float len = Mathf.Sqrt(dx * dx + dy * dy);
//
//			// Early out on tiny lines to avoid divide by zero.
//			// Plus what's the point of drawing a line 1/1000th of a pixel long??
//			if (len < 0.001f)
//			{
//				return;
//			}
//
//			// Pick texture and material (and tweak width) based on anti-alias setting.
//			Texture2D tex;
//			Material mat;
//			if (antiAlias)
//			{
//				// Multiplying by three is fine for anti-aliasing width-1 lines, but make a wide "fringe"
//				// for thicker lines, which may or may not be desirable.
//				width = width * 3.0f;
//				tex = aaLineTex;
//				mat = blendMaterial;
//			}
//			else
//			{
//				tex = lineTex;
//				mat = blitMaterial;
//			}
//
//			float wdx = width * dy / len;
//			float wdy = width * dx / len;
//
//			Matrix4x4 matrix = Matrix4x4.identity;
//			matrix.m00 = dx;
//			matrix.m01 = -wdx;
//			matrix.m03 = pointA.x + 0.5f * wdx;
//			matrix.m10 = dy;
//			matrix.m11 = wdy;
//			matrix.m13 = pointA.y - 0.5f * wdy;
//
//			// Use GL matrix and Graphics.DrawTexture rather than GUI.matrix and GUI.DrawTexture,
//			// for better performance. (Setting GUI.matrix is slow, and GUI.DrawTexture is just a
//			// wrapper on Graphics.DrawTexture.)
//			GL.PushMatrix();
//			GL.MultMatrix(matrix);
//			//Graphics.DrawTexture(lineRect, tex, lineRect, 0, 0, 0, 0, color, mat);
//			//Replaced by:
//			GUI.color = color;//this and...
//			GUI.DrawTexture( lineRect, tex );//this
//
//			GL.PopMatrix();
//		}
//
//		public static void DrawCircle(Vector2 center, int radius, Color color, float width, int segmentsPerQuarter) {
//			DrawCircle(center, radius, color, width, false, segmentsPerQuarter);
//		}
//
//		public static void DrawCircle(Vector2 center, int radius, Color color, float width, bool antiAlias, int segmentsPerQuarter) {
//			float rh = (float)radius / 2;
//
//			Vector2 p1 = new Vector2(center.x, center.y - radius);
//			Vector2 p1_tan_a = new Vector2(center.x - rh, center.y - radius);
//			Vector2 p1_tan_b = new Vector2(center.x + rh, center.y - radius);
//
//			Vector2 p2 = new Vector2(center.x + radius, center.y);
//			Vector2 p2_tan_a = new Vector2(center.x + radius, center.y - rh);
//			Vector2 p2_tan_b = new Vector2(center.x + radius, center.y + rh);
//
//			Vector2 p3 = new Vector2(center.x, center.y + radius);
//			Vector2 p3_tan_a = new Vector2(center.x - rh, center.y + radius);
//			Vector2 p3_tan_b = new Vector2(center.x + rh, center.y + radius);
//
//			Vector2 p4 = new Vector2(center.x - radius, center.y);
//			Vector2 p4_tan_a = new Vector2(center.x - radius, center.y - rh);
//			Vector2 p4_tan_b = new Vector2(center.x - radius, center.y + rh);
//
//			DrawBezierLine(p1, p1_tan_b, p2, p2_tan_a, color, width, antiAlias, segmentsPerQuarter);
//			DrawBezierLine(p2, p2_tan_b, p3, p3_tan_b, color, width, antiAlias, segmentsPerQuarter);
//			DrawBezierLine(p3, p3_tan_a, p4, p4_tan_b, color, width, antiAlias, segmentsPerQuarter);
//			DrawBezierLine(p4, p4_tan_a, p1, p1_tan_a, color, width, antiAlias, segmentsPerQuarter);
//		}
//
//		// Other than method name, DrawBezierLine is unchanged from Linusmartensson's original implementation.
//		public static void DrawBezierLine(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float width, bool antiAlias, int segments)
//		{
//			Vector2 lastV = CubeBezier(start, startTangent, end, endTangent, 0);
//			for (int i = 1; i < segments + 1; ++i)
//			{
//				Vector2 v = CubeBezier(start, startTangent, end, endTangent, i/(float)segments);
//				Drawing.DrawLine(lastV, v, color, width, antiAlias);
//				lastV = v;
//			}
//		}
//
//
//		private static Vector2 CubeBezier(Vector2 s, Vector2 st, Vector2 e, Vector2 et, float t)
//		{
//			float rt = 1 - t;
//			return rt * rt * rt * s + 3 * rt * rt * t * st + 3 * rt * t * t * et + t * t * t * e;
//		}
//
//		// This static initializer works for runtime, but apparently isn't called when
//		// Editor play mode stops, so DrawLine will re-initialize if needed.
//		static Drawing()
//		{
//			Initialize();
//		}
//
//		private static void Initialize()
//		{
//			if (lineTex == null)
//			{
//				lineTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
//				lineTex.SetPixel(0, 1, Color.white);
//				lineTex.Apply();
//			}
//			if (aaLineTex == null)
//			{
//				// TODO: better anti-aliasing of wide lines with a larger texture? or use Graphics.DrawTexture with border settings
//				aaLineTex = new Texture2D(1, 3, TextureFormat.ARGB32, false);
//				aaLineTex.SetPixel(0, 0, new Color(1, 1, 1, 0));
//				aaLineTex.SetPixel(0, 1, Color.white);
//				aaLineTex.SetPixel(0, 2, new Color(1, 1, 1, 0));
//				aaLineTex.Apply();
//			}
//
//			// GUI.blitMaterial and GUI.blendMaterial are used internally by GUI.DrawTexture,
//			// depending on the alphaBlend parameter. Use reflection to "borrow" these references.
//			blitMaterial = (Material)typeof(GUI).GetMethod("get_blitMaterial", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
//			blendMaterial = (Material)typeof(GUI).GetMethod("get_blendMaterial", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
//		}
//	}
