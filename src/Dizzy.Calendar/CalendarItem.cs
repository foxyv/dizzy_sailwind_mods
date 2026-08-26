using UnityEngine;

namespace Dizzy.Calendar
{
    /// <summary>
    /// Wall-mountable calendar that shows the live Sailwind day on its face.
    /// </summary>
    public class CalendarItem : ShipItem
    {
        public const int PrefabIndex = 930;

        private TextMesh _dateText;
        private bool _hooked;
        private int _lastShownDay = int.MinValue;

        public void BindDateText(TextMesh dateText)
        {
            _dateText = dateText;
        }

        public override void OnLoad()
        {
            base.OnLoad();
            EnsureDayHook();
            RefreshDateDisplay(force: true);
        }

        private void OnEnable()
        {
            EnsureDayHook();
            RefreshDateDisplay(force: true);
        }

        private void OnDisable()
        {
            UnhookDay();
        }

        public override void ExtraFixedUpdate()
        {
            base.ExtraFixedUpdate();
            // Fallback if OnNewDay was missed (e.g. load mid-day). Do not define Update() —
            // ShipItem.Update is private and handles wallAttachment; hiding it would break hanging.
            if (GameState.day != _lastShownDay)
                RefreshDateDisplay(force: true);
        }

        public override void UpdateLookText()
        {
            base.UpdateLookText();
            string dayLine = FormatDayLabel();
            if (string.IsNullOrEmpty(lookText))
                lookText = dayLine;
            else if (!lookText.Contains(dayLine))
                lookText = lookText + "\n" + dayLine;
        }

        private void EnsureDayHook()
        {
            if (_hooked)
                return;

            Sun.OnNewDay += HandleNewDay;
            _hooked = true;
        }

        private void UnhookDay()
        {
            if (!_hooked)
                return;

            Sun.OnNewDay -= HandleNewDay;
            _hooked = false;
        }

        private void HandleNewDay()
        {
            RefreshDateDisplay(force: true);
        }

        public void RefreshDateDisplay(bool force = false)
        {
            int day = GameState.day;
            if (!force && day == _lastShownDay)
                return;

            _lastShownDay = day;
            string label = FormatDayLabel();
            if (_dateText != null)
                _dateText.text = label;
        }

        public static string FormatDayLabel()
        {
            return "Day " + GameState.day;
        }
    }
}
