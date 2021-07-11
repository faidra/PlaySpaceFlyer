using UnityEngine;
using UniRx;
using UnityEngine.UI;
using Valve.VR;
using System.Linq;
using System;
using Valve.Newtonsoft.Json.Utilities;

public class TrackerSelector : MonoBehaviour
{
    [SerializeField] Dropdown Dropdown;
    [SerializeField] uint prefered;

    uint[] _trackingReferenceIds;
    ReplaySubject<uint> _currentSelected = new ReplaySubject<uint>(1);

    void Update()
    {
        var ids = new uint[OpenVR.k_unMaxTrackedDeviceCount];
        var count = OpenVR.System.GetSortedTrackedDeviceIndicesOfClass(ETrackedDeviceClass.GenericTracker, ids, 0u);
        if (_trackingReferenceIds.Length != count + 1)
        {
            _trackingReferenceIds = ids.Take((int) count).Prepend(0u).ToArray();
            Dropdown.options = _trackingReferenceIds.Select(id => new Dropdown.OptionData(id.ToString())).ToList();
            var preferedIndex = _trackingReferenceIds.IndexOf(i => i == prefered);
            if (preferedIndex >= 0) Dropdown.value = preferedIndex;
        }
    }

    void Start()
    {
        _trackingReferenceIds = new[] {0u};
        Dropdown.onValueChanged.AsObservable()
            .Where(i => i < _trackingReferenceIds.Length)
            .Select(i => _trackingReferenceIds[i])
            .Subscribe(_currentSelected)
            .AddTo(this);
    }

    public IObservable<uint> SelectedTrackingReferenceIdAsObservable() => _currentSelected;
}