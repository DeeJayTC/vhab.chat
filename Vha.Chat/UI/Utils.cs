/*
* Vha.Chat
* Copyright (C) 2009-2010 Remco van Oosterhout
* See Credits.txt for all aknowledgements.
*
* This program is free software; you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation; version 2 of the License only.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this program; if not, write to the Free Software
* Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
* USA
*/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Vha.Chat.UI
{
    public static class Utils
    {
        public delegate void FormDelegate(Form form);

        public static void InvokeHide(Form form)
        {
            if (form.InvokeRequired)
            {
                form.Invoke(new FormDelegate(InvokeHide), form);
                return;
            }
            form.Visible = false;
        }

        public static void InvokeShow(Form form)
        {
            if (form.InvokeRequired)
            {
                form.Invoke(new FormDelegate(InvokeShow), form);
                return;
            }
            form.Visible = true;
        }

        public static void InvokeClose(Form form)
        {
            if (form.InvokeRequired)
            {
                form.Invoke(new FormDelegate(InvokeClose), form);
                return;
            }
            form.Close();
        }

        public delegate void InvokeShowDelegate(Form parent, Form form);
        public static void InvokeShow(Form parent, Form form)
        {
            if (form.InvokeRequired)
            {
                form.Invoke(new InvokeShowDelegate(InvokeShow), new Object[] { parent, form });
                return;
            }
            // Show form (to initial default values)
            form.Show(parent);
            int offsetX = -(form.Size.Width / 2);
            int offsetY = -(form.Size.Height / 2);
            // Move window if needed
            if (form.StartPosition == FormStartPosition.CenterParent)
            {
                int centerX = parent.Location.X + (parent.Size.Width / 2);
                int centerY = parent.Location.Y + (parent.Size.Height / 2);
                Point target = new Point(
                    centerX + offsetX,
                    centerY + offsetY);
                Move(form, Screen.FromControl(parent), target);
            }
            if (form.StartPosition == FormStartPosition.CenterScreen)
            {
                Screen targetScreen = Screen.FromControl(parent);
                int centerX = targetScreen.WorkingArea.Left + (targetScreen.WorkingArea.Size.Width / 2);
                int centerY = targetScreen.WorkingArea.Top + (targetScreen.WorkingArea.Size.Height / 2);
                Point target = new Point(
                    centerX + offsetX,
                    centerY + offsetY);
                Move(form, targetScreen, target);
            }
            else if (form.StartPosition == FormStartPosition.WindowsDefaultLocation)
            {
                // Check if this form has been spawned on the wrong screen
                Screen parentScreen = Screen.FromControl(parent);
                Screen formScreen = Screen.FromControl(form);
                if (!parentScreen.Equals(formScreen))
                {
                    // Move the form to the right screen
                    form.Location = new Point(
                        (form.Location.X - formScreen.WorkingArea.Left) + parentScreen.WorkingArea.Left,
                        (form.Location.Y - formScreen.WorkingArea.Top) + parentScreen.WorkingArea.Top);
                }
            }
        }

        static public void Move(Form form, Screen screen, Point location)
        {
            if (screen != null)
            {
                location = new Point(
                    screen.WorkingArea.Left > location.X ? screen.WorkingArea.Left : location.X,
                    screen.WorkingArea.Top > location.Y ? screen.WorkingArea.Top : location.Y
                    );
            }
            form.Location = location;
        }
    }
}
