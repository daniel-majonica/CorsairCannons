using EmptySkull.Management;
using EmptySkull.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetPositionManager : ManagerModule
{
    [SerializeField] private NullableFloat _updateRateSeconds = new NullableFloat() { SetToNull = true };


    private HashSet<ITargetable> _registeredTargets;
    private Dictionary<(ITargetable, ITargetable), float> _distancesSqr;
    private HashSet<ITargetable> _calculatedTargetsBuffer;

    private Dictionary<(ITargetable, ITargetable), float> _distanceMemorization;

    private float _updateTimer;


    protected virtual void Start()
    {
        CalculateDistancesSqr();
    }

    protected virtual void Update()
    {
        if(_updateRateSeconds.TryGetValue(out float updateRate))
        {
            _updateTimer += Time.deltaTime;
            if (_updateTimer < updateRate)
                return;

            _updateTimer -= updateRate;
        }

        CalculateDistancesSqr();
    }

    protected override void OnInitialize()
    {
        _registeredTargets = new HashSet<ITargetable>();
        _distancesSqr = new Dictionary<(ITargetable, ITargetable), float>();
        _calculatedTargetsBuffer = new HashSet<ITargetable>();
        _distanceMemorization = new Dictionary<(ITargetable, ITargetable), float>();

        base.OnInitialize();
    }


    #region Registration

    public void RegisterTarget(ITargetable target)
    {
        _registeredTargets.Add(target);

        CalculateDistancesSqr();
    }

    public void DeregisterTarget(ITargetable target)
    {
        _registeredTargets.Remove(target);

        CalculateDistancesSqr();
    }

    public bool IsRegisteredTarget(ITargetable target)
        => _registeredTargets.Contains(target);

    public void DeregisterAllTargets()
        => _registeredTargets.Clear();

    #endregion

    #region Access

    public ITargetable[] GetTargetsInRangeSqr(ITargetable origin, float rangeSqr)
    {
        if (!_registeredTargets.Contains(origin))
            throw new ArgumentException("The target was not yet registered.");

        HashSet<ITargetable> result = new HashSet<ITargetable>();
        foreach(ITargetable compareTarget in _registeredTargets)
        {
            if (compareTarget == origin)
                continue;

            if(GetDistanceBetweenSqr(origin, compareTarget) <= rangeSqr)
                result.Add(compareTarget);
        }

        return result.ToArray();


    }

    public ITargetable[] GetTargetsInRange(ITargetable origin, float range)
        => GetTargetsInRangeSqr(origin, range * range);

    public float GetDistanceBetweenSqr(ITargetable a, ITargetable b)
    {
        if (!_registeredTargets.Contains(a) || !_registeredTargets.Contains(b))
            throw new ArgumentException("One of the compared targets was not yet registered.");

        if (a == b)
            return 1f;

        if (_distancesSqr.TryGetValue((a, b), out float distAB))
            return distAB;

        if (_distancesSqr.TryGetValue((b, a), out float distBA))
            return distBA;

        throw new InvalidOperationException("Cannot read target distances. Distances might not yet be calculated.");
    }

    public float GetDistanceBetween(ITargetable a, ITargetable b)
    {
        if(_distanceMemorization.TryGetValue((a, b), out float memorizedDistanceAB))
            return memorizedDistanceAB;

        if (_distanceMemorization.TryGetValue((b, a), out float memorizedDistanceBA))
            return memorizedDistanceBA;

        float distance = Mathf.Sqrt(GetDistanceBetweenSqr(a, b));
        _distanceMemorization.Add((a, b), distance);
        return distance;
    }

    #endregion

    private void CalculateDistancesSqr()
    {
        _calculatedTargetsBuffer.Clear();
        _distancesSqr.Clear();

        foreach (ITargetable target in _registeredTargets)
        {
            foreach(ITargetable compareTarget in _registeredTargets)
            {
                if (target == compareTarget || _calculatedTargetsBuffer.Contains(compareTarget))
                    continue;

                _distancesSqr.Add((target, compareTarget), (compareTarget.WorldPosition - target.WorldPosition).sqrMagnitude);
            }
            _calculatedTargetsBuffer.Add(target);
        }
    }
}
