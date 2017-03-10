using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechAction : MonoBehaviour {

	KeywordRecognizer keywordRecognizer = null;
	Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

	Vector3 originalPosition;

	// Use this for initialization
	void Start () {
		Debug.Log("Start is called");
		originalPosition = this.transform.localPosition;

		keywords.Add("Show me", () =>
			{
				// Call the OnReset method on every descendant object.
				this.BroadcastMessage("OnReset");
			});

		keywords.Add ("This is enough", () => {
			this.BroadcastMessage ("OnDrop");
		});

		keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

		// Register a callback for the KeywordRecognizer and start recognizing!
		keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
		keywordRecognizer.Start();
	}
	
	private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
	{
		Debug.Log(args.text + " is recognized");
		System.Action keywordAction;
		if (keywords.TryGetValue(args.text, out keywordAction))
		{
			keywordAction.Invoke();
		}
	}

	void OnDrop()
	{
		Debug.Log("OnDrop is called");
		// If the sphere has no Rigidbody component, add one to enable physics.
		if (!this.GetComponent<Rigidbody>())
		{
			var rigidbody = this.gameObject.AddComponent<Rigidbody>();
			rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
		}
	}

	void OnReset()
	{
		Debug.Log("OnReset is called");
		// If the sphere has a Rigidbody component, remove it to disable physics.
		var rigidbody = this.GetComponent<Rigidbody>();
		if (rigidbody != null)
		{
			DestroyImmediate(rigidbody);
		}

		// Put the sphere back into its original local position.
		this.transform.localPosition = originalPosition;
	}
}
