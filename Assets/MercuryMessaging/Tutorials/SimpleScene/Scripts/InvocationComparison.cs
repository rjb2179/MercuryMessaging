﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;
using UnityEngine.EventSystems;

using MercuryMessaging;

public interface ICustomMessageTarget : IEventSystemHandler
{
	// functions that can be called via the messaging system
	void SimpleFunction();
}

public class InvocationComparison : MmBaseResponder, ICustomMessageTarget {

	/// <summary>
	/// Various testing modes supported by the invocation comparison system
	/// </summary>
	public enum TestingMode {Control = 0, Mercury, SendMessage, Event, Execute};

	/// <summary>
	/// Event delegate for event test.
	/// </summary>
	public delegate void EventActionTest();


	/// <summary>
	/// This object's relay node.
	/// </summary>
	MmRelayNode myNode;

	/// <summary>
	/// Number of repetitions for the test.
	/// </summary>
	public int Repetitions = 1000;

	/// <summary>
	/// The stop watch.
	/// </summary>
	public System.Diagnostics.Stopwatch stopWatch;

	//Another stop watch that includes loop time.
	public System.Diagnostics.Stopwatch totalWatch;

	/// <summary>
	/// The Mercury block to use in the routing test.
	/// </summary>
	MmMetadataBlock mmBlock;

	/// <summary>
	/// Simple lock to use in some tests.
	/// </summary>
	public bool simpleLock = false;

	/// <summary>
	/// Occurs when on event test.
	/// </summary>
	public event EventActionTest OnEventTest;

	public StringBuilder Results;


	/// <summary>
	/// Start this instance.
	/// </summary>
	public override void Start () {

		Results = new StringBuilder ();

		//Debug.Log ("Set to repeat call: " + Repetitions + " times.");
		Results.AppendLine ("Set to repeat call: " + Repetitions + " times.");

		//Prepare the relay node for the mercury test
		myNode = GetRelayNode ();
		mmBlock = new MmMetadataBlock (MmLevelFilter.Self, MmActiveFilter.All);

		//Prepare the stopwatch we'll use across tests. It will be reset each time.
		stopWatch = new System.Diagnostics.Stopwatch ();

		//Prepare the total time stop watch. 
		totalWatch = new System.Diagnostics.Stopwatch ();

		OnEventTest += SimpleFunction;
	}

	/// <summary>
	/// Press Space to run tests.
	/// </summary>
	public override void Update()
	{
		if(Input.GetKey (KeyCode.Space))
		{
			Results = new StringBuilder ();
			TimingTest ();
		}
	}

	/// <summary>
	/// Run through all tests
	/// </summary>
	public void TimingTest()
	{
		foreach (TestingMode mode in Enum.GetValues(typeof(TestingMode)))
		{
			TimingFunction (mode);
		}

		Debug.Log (Results.ToString ());
	}

	/// <summary>
	/// Timing Function.
	/// </summary>
	/// <param name="state">State.</param>
	public void TimingFunction(TestingMode state)
	{
		stopWatch.Reset ();

		totalWatch.Reset ();

		Debug.Log (state.ToString () + " test.");
		Results.AppendLine (state.ToString () + " test.");

		switch(state)
		{
			case TestingMode.Control:
				TestFunction ();
				break;
			case TestingMode.Mercury:
				TestMercury ();
				break;
			case TestingMode.SendMessage:
				TestSendMessage ();
				break;
			case TestingMode.Event:
				TestEvent ();
				break;
			case TestingMode.Execute:
			TestExecute ();
				break;
		}

//		Debug.Log (state.ToString () + " Total Time: " + 
//			stopWatch.ElapsedMilliseconds + " milliseconds.");
//		Debug.Log (state.ToString () + " Avg Time: " + 
//			stopWatch.ElapsedMilliseconds/((float)Repetitions) + " milliseconds.");

		Results.AppendLine (//state.ToString () + " Total Time: " + 
			stopWatch.ElapsedMilliseconds.ToString ());// + " milliseconds.");
		Results.AppendLine (//state.ToString () + " Avg Time: " + 
			(stopWatch.ElapsedMilliseconds / ((float)Repetitions)).ToString ());// + " milliseconds.");
		Results.AppendLine (//state.ToString () + " Total Time: " + 
			stopWatch.ElapsedTicks.ToString ());// + " ticks.");
		Results.AppendLine (//state.ToString () + " Avg Time: " + 
			(stopWatch.ElapsedTicks / ((float)Repetitions)).ToString ());// + " ticks.");
	}

	/// <summary>
	/// Timing test of standard function invocation in Unity.
	/// </summary>
	public void TestFunction()
	{
		int counter = 0;
		while (counter < Repetitions)
		{
			stopWatch.Start ();
			SimpleFunction ();

			counter++;
		}
	}

	/// <summary>
	/// Mercury timing test.
	/// </summary>
	public void TestMercury()
	{
		int counter = 0;
		while (counter < Repetitions)
		{
			stopWatch.Start ();
			myNode.MmInvoke (MmMethod.Initialize, mmBlock);

			counter++;
		}
	}

	/// <summary>
	/// Unity send message timing test.
	/// </summary>
	public void TestSendMessage()
	{
		int counter = 0;
		while (counter < Repetitions)
		{
			if (!simpleLock) {
				simpleLock = true;

				stopWatch.Start ();
				this.gameObject.SendMessage ("SimpleFunction");

				counter++;
			}
		}
	}

	/// <summary>
	/// Unity event timing test.
	/// </summary>
	public void TestEvent()
	{
		int counter = 0;
		while (counter < Repetitions)
		{
			if (!simpleLock) {
				simpleLock = true;

				stopWatch.Start ();
				OnEventTest ();

				counter++;
			}
		}
	}

	/// <summary>
	/// Unity execute timing test.
	/// </summary>
	public void TestExecute()
	{
		int counter = 0;
		while (counter < Repetitions)
		{
			if (!simpleLock) {
				simpleLock = true;

				stopWatch.Start ();

				ExecuteEvents.Execute<InvocationComparison>(this.gameObject, null, (x, y)=>x.SimpleFunction());

				counter++;
			}
		}
	}

	/// <summary>
	/// Empty Function to use for function invocation.
	/// </summary>
	public void SimpleFunction()
	{
		//Executable code
		//Debug.Log ("Function Called.");
		//int i = 3 + 4;

		stopWatch.Stop ();

		simpleLock = false;
	}

	/// <summary>
	/// Overrides the base MmInvoke
	/// Here we just stop the stopwatch. But we could throw in a switch (to represent normal usage) too.
	/// </summary>
	public override void MmInvoke (MmMessageType msgType, MercuryMessaging.Message.MmMessage message)
	{
		//base.MmInvoke (msgType, message);

		//Executable code
		//Debug.Log ("Function Called.");
		//int i = 3 + 4;

		stopWatch.Stop ();

		simpleLock = false;
	}
}
