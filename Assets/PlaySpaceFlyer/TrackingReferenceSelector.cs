﻿using UnityEngine;
using UniRx;
using UnityEngine.UI;
using Valve.VR;
using System.Linq;
using System;

public class TrackingReferenceSelector : MonoBehaviour
{
    [SerializeField]
    Dropdown Dropdown;

    uint[] _trackingReferenceIds;
    ReplaySubject<uint> _currentSelected = new ReplaySubject<uint>(1);

    void Start()
    {
        var ids = new uint[OpenVR.k_unMaxTrackedDeviceCount];
        var count = OpenVR.System.GetSortedTrackedDeviceIndicesOfClass(ETrackedDeviceClass.TrackingReference, ids, 0u);
        _trackingReferenceIds = ids.Take((int)count).ToArray();
        Dropdown.options = _trackingReferenceIds.Select(id => new Dropdown.OptionData(id.ToString())).ToList();
        Dropdown.onValueChanged.AsObservable()
            .Select(i => _trackingReferenceIds[i])
            .Subscribe(_currentSelected)
            .AddTo(this);
    }

    public IObservable<uint> SelectedTrackingReferenceIdAsObservable() => _currentSelected;
}