using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TypeRacer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum TypeState
        {
            Idle,
            Restarted,
            Playing
        }

        private string _text;
        private int _wordIndex;
        private Stopwatch _stopWatch = new Stopwatch();
        private List<Word> _words = new List<Word>();

        private TypeState _state = TypeState.Idle;

        public MainWindow()
        {
            InitializeComponent();

            _text = txtSource.Text;
            Restart();
        }

        private void txtSource_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtText != null)
            {
                _text = txtSource.Text;
                _text = string.Join("", _text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                txtText.Text = _text;
            }
        }

        private void txtType_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_state == TypeState.Restarted)
            {
                _state = TypeState.Playing;
                _stopWatch.Restart();
            }

            ColorWord();
            UpdateStats();
        }

        private void UpdateStats()
        {
            int wordsTyped = _wordIndex;
            int charactersTyped;

            if (_wordIndex == _words.Count)
            {
                charactersTyped = _text.Length;
            }
            else
            {
                charactersTyped = _words[_wordIndex].Position;
            }

            int wpm;

            if (_stopWatch.Elapsed.TotalSeconds == 0)
            {
                wpm = 0;
            }
            else
            {
                wpm = (int)((60 / _stopWatch.Elapsed.TotalSeconds) * ((double)charactersTyped / 5));
            }

            txtWPM.Text = wpm.ToString();
        }

        private void ColorWord()
        {
            var run1 = new Run();
            var run2 = new Run();
            var run3 = new Run();

            var currentWord = _words[_wordIndex];

            run1.Text = _text.Substring(0, currentWord.Position);

            run2.Text = currentWord.Text;
            run2.TextDecorations.Add(TextDecorations.Underline);

            if (_words.Count > _wordIndex + 1)
            {
                run3.Text = _text.Substring(_words[_wordIndex + 1].Position - 1);
            }

            if (txtType.Text == currentWord.Text + " ")
            {                
                txtType.Text = string.Empty;
                _wordIndex++;

                if (_wordIndex == _words.Count)
                {
                    Finish();
                }
                else
                {
                    ColorWord();
                }

                return;
            }

            if (currentWord.Text.StartsWith(txtType.Text))
            {
                run2.Foreground = Brushes.Green;
                txtType.Background = Brushes.White;
            }
            else
            {
                run2.Foreground = Brushes.Red;
                txtType.Background = Brushes.LightCoral;
            }

            txtText.Inlines.Clear();

            txtText.Inlines.Add(run1);
            txtText.Inlines.Add(run2);
            txtText.Inlines.Add(run3);
        }

        private void Finish()
        {
            txtType.IsReadOnly = true;
            _stopWatch.Stop();
            _state = TypeState.Idle;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Restart();
        }

        private void Restart()
        {
            txtType.IsReadOnly = false;
            _state = TypeState.Restarted;
            _wordIndex = 0;

            var words = _text.Split(' ');
            _words.Clear();
            var position = 0;

            for (int i = 0; i < words.Length; i++)
            {
                _words.Add(new Word(words[i], position));
                position += words[i].Length + 1;
            }

            txtText.Text = _text;
            txtType.Text = string.Empty;
            txtType.Focus();

            ColorWord();
        }
    }

    public class Word
    {
        public Word(string text, int position)
        {
            Text = text;
            Position = position;
        }

        public string Text { get; private set; }
        public int Position { get; private set; }

        public int Length
        {
            get { return Text.Length; }
        }
    }
}
