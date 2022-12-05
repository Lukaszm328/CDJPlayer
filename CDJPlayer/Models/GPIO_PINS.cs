namespace DjPlayer.Model
{
    public class GPIO_PINS
    {
        #region BUTTONS PINS
        public int PLAY_PAUSE_BUTTON_PIN  { get { return 20; } }
        public int CUE_BUTTON_PIN { get { return 21; } }
        public int NEXT_BUTTON_PIN { get { return 4; } }
        public int BACK_BUTTON_PIN { get { return 17; } }
        public int SEARCH_PLUS_BUTTON_PIN { get { return 27; }}
        public int SEARCH_MINUS_BUTTON_PIN { get { return 22; } }
        public int JET_BUTTON_PIN { get { return 10; } }
        public int ZIP_BUTTON_PIN { get { return 9; } }
        public int WAH_BUTTON_PIN { get { return 11; } }
        public int HOLD_BUTTON { get { return 24; } }
        public int AUTOCUE_BUTTON_PIN { get { return 5; } }
        public int REMOVE_DISC_BUTTON_PIN { get { return 6; } }
        public int TEMPO_BUTTON_PIN { get { return 13; } }
        #endregion

        #region LED's PINS
        public int PLAY_PAUSE_LED_PIN { get { return 19; } }
        public int CUE_LED_PIN { get { return 26; } }
        #endregion

        #region I2C Alert (Tempo)
        public int ADS_ALERT_PIN { get { return 23; } }
        #endregion

        #region JOG ENCODER VALUE PINS
        public int CLK_JOG_PIN { get { return 18; } }
        public int DT_JOG_PIN { get { return 25; } }
        #endregion

        #region PITCH VALUE PINS
        public int PITCH1_PIN { get { return 24; } }
        public int PITCH2_PIN { get { return 25; } }
        public int PITCH3_PIN { get { return 8; } }
        #endregion

        #region BROWSER ENCODER
        public int CLK_BROWSER_ENCODER_PIN { get { return 15; } }
        public int DT_BROWSER_ENCODER_PIN { get { return 14; } }
        public int BTN_BROWSER_ENCODER_PIN { get { return 7; } }
        #endregion
    }
}