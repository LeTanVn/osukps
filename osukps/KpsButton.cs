using System;
using System.Drawing;
using System.Windows.Forms;

namespace osukps {

	public class KpsButton : Panel {

		private Label label;
		private Label lblSingleKps;
		private byte[] kpsArray = new byte[10];
		private byte kpsIndex;
		private int kpsMax;
		private Timer kpsTimer;
		public IKeyHandler keyhandler;
		private int colortimer;
		private int key;
		public KpsButtonColor color;
		public event EventHandler settingChangedEvent;

		public KpsButton(int position) {
			color = new KpsButtonColor();
			Visible = true;
			AutoSize = false;
			Size = new Size(36, 54);
			Location = new Point(40 * position, 0);
			createLabel();
			
			kpsTimer = new Timer();
			kpsTimer.Interval = 100;
			kpsTimer.Tick += KpsTimer_Tick;
			kpsTimer.Start();
			
			keyhandler = NoKeyHandler.Get();
			UpdateColor();
		}

		public void KeySetup(int k) {
			key = k;
			keyhandler = new DefKeyHandler(k);
		}

		public void LabelSetup(string t) {
			label.Text = t;
		}

		public void ActiveColorSetup(int c) {
			color.active = Color.FromArgb(c);
		}

		public void InactiveColorSetup(int c) {
			color.inactive = Color.FromArgb(c);
			if (lblSingleKps != null) {
				lblSingleKps.ForeColor = frmMain.SingleKpsColor;
			}
		}

		public void createLabel() {
			label = new Label();
			label.Visible = true;
			label.AutoSize = false;
			label.Size = new Size(36, 36);
			label.Location = new Point(0, 0);
			label.Text = "";
			label.TextAlign = ContentAlignment.MiddleCenter;
			label.ForeColor = Color.White;
			label.Click += KpsButton_Click;
			label.ForeColor = frmMain.FgColor;
			FontHandler.labels.Add(label);
			Controls.Add(label);
			
			lblSingleKps = new Label();
			lblSingleKps.Visible = true;
			lblSingleKps.AutoSize = false;
			lblSingleKps.Size = new Size(36, 18);
			lblSingleKps.Location = new Point(0, 36);
			lblSingleKps.Text = "0";
			lblSingleKps.TextAlign = ContentAlignment.MiddleCenter;
			lblSingleKps.ForeColor = frmMain.SingleKpsColor;
			lblSingleKps.Font = new Font("Tahoma", 8, FontStyle.Bold);
			Controls.Add(lblSingleKps);
		}

		private void KpsButton_Click(object sender, EventArgs e) {
			DialogPositioner.From(FindForm(), PointToScreen(new Point(Width / 2, Height / 2)));
			IKeyHandler newHandler = frmGetKey.ShowDialogAndGetKeyHandler(color, key, label.Text);
			if (newHandler == null) {
				return;
			}
			keyhandler = newHandler;
			key = frmGetKey.yourkey(); //get my key id
			frmGetKey.UpdateLabel(label);
			if (settingChangedEvent != null) {
				settingChangedEvent(null, null);
			}
		}

		//for save key id and label text
		public int mykey() {
			return key;
		}

		public string mystring() {
			return label.Text;
		}

		public int myactivecolor() {
			return color.active.ToArgb();
		}

		public int myinactivecolor() {
			return color.inactive.ToArgb();
		}

		public byte Process() {
			byte result = keyhandler.Handle();
			if (result == 1) {
				colortimer = 255;
				result = 1;
			} else {
				colortimer = Math.Max(colortimer - 15, 0);
			}
			UpdateColor();
			return result;
		}

		public void UpdateColor() {
			float f = colortimer / 255f;
			int r = color.inactive.R + (int) (f * (color.active.R - color.inactive.R));
			int g = color.inactive.G + (int) (f * (color.active.G - color.inactive.G));
			int b = color.inactive.B + (int) (f * (color.active.B - color.inactive.B));
			label.BackColor = Color.FromArgb(255, r, g, b);
		}

		public void OnForeColorChange() {
			label.ForeColor = frmMain.FgColor;
		}

		public void OnSingleKpsColorChange() {
			if (lblSingleKps != null) {
				lblSingleKps.ForeColor = frmMain.SingleKpsColor;
			}
		}

		private void KpsTimer_Tick(object sender, EventArgs e) {
			if (++kpsIndex >= 10) {
				kpsIndex = 0;
			}
			kpsArray[kpsIndex] = 0;
			UpdateSingleKpsLabel();
		}

		public void AddKps() {
			kpsArray[kpsIndex]++;
			UpdateSingleKpsLabel();
		}

		private void UpdateSingleKpsLabel() {
			int currentKps = 0;
			for (int i = 0; i < 10; i++) {
				currentKps += kpsArray[i];
			}
			
			if (currentKps > kpsMax) {
				kpsMax = currentKps;
			}
			
			lblSingleKps.Text = currentKps.ToString();
		}

	}
}
