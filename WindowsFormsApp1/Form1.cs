using System;
using System.Collections.Generic;
using System.Drawing;

using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class MainForm : Form
    {

        private int score = 0;
        private int level = 1;
        private int rows = 4;
        private int cols = 3;
        private int DefaultCardSize = 200;
        private  int CardWidth = 0;
       
        private const int HideCardsDelay = 1000;

        private List<PictureBox> cards;
        private PictureBox firstCard;
        private int matchedCardsCount;
        static string basePath = "D:\\c#\\WindowsFormsApp1\\WindowsFormsApp1\\Recources";
        private List<string> imagePaths = new List<string>
        {
              basePath+ "\\card1.jpg",
              basePath+ "\\card2.jpg",
              basePath+ "\\card3.jpg",
              basePath+ "\\card4.jpg",
              basePath+ "\\card5.jpg",
              basePath+ "\\card10.jpg",
            // Add paths to other images here
        };

        public MainForm()
        {
            InitializeComponent();
            score = 0;
            InitializeGame();
           
        }

        private void InitializeGame()
        {
          
            UpdateLevelScore(); // Khởi tạo điểm ban đầ
            Controls.Add(label1);
            cards = new List<PictureBox>();
            matchedCardsCount = 0;
            // Create and shuffle the cards
            int cardCount = rows * cols; 
            CardWidth = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(DefaultCardSize / cols)));
            for (int i = 0; i < cardCount / 2; i++)
            {

                cards.Add(CreateCard(i));
                cards.Add(CreateCard(i));
              
            }

            ShuffleCards();
          

            // Add the cards to the form
            int cardIndex = 0;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    int x = ((cardIndex % cols) * (CardWidth + 10)) ;
                    int y = ((cardIndex / cols) * (CardWidth + 10));

                    PictureBox card = cards[cardIndex];
                    card.Location = new Point(x ,y);
                    card.Click += Card_Click;
                    Controls.Add(card);
                    cardIndex++;
                  
                }
            }
           
        }
        // hàm tạo card item
        private PictureBox CreateCard(int imageIndex)
        {
            int imageSize = DefaultCardSize / rows; // Kích thước hình ảnh cho mỗi ô lưới
            int imageCount = rows * cols; // Tổng số hình ảnh trong lưới
            PictureBox card = new PictureBox();
            card.Size = new Size(CardWidth, CardWidth);
            card.Tag = imagePaths[imageIndex]; 
            card.SizeMode = PictureBoxSizeMode.StretchImage;

            string imagePath = imagePaths[imageIndex];
            Image img = Image.FromFile("D:\\c#\\WindowsFormsApp1\\WindowsFormsApp1\\Recources\\imagecard2.jpg");
            card.Image = ResizeImageToGrid(img, imageSize, imageSize);
            return card;
        }
        // hàm xáo trộn các thẻ hình
        private void ShuffleCards()
        {
            Random random = new Random();
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                PictureBox value = cards[k];
                cards[k] = cards[n];
                cards[n] = value;
            }
        }
         
        // hàm thay đổi kích thước hình ảnh để phù hợp với form
        private Image ResizeImageToGrid(Image image, int width, int height)
        {
            Bitmap resizedImage = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(resizedImage))
            {
                graphics.DrawImage(image, 0, 0, width, height);
            }
            return resizedImage;
        }
        // hàm cập nhật điểm và hiện thị
       
        private void UpdateLevelScore()
        {
            score += 100;
            label1.Text = "Score: " + (score-100).ToString();
        }
        // hàm xủ lí click thẻ hình 
        private bool isProcessing = false; // Biến kiểm tra xem có đang xử lý các click trước đó hay không
    

        private void Card_Click(object sender, EventArgs e)
        {
            if (isProcessing)
            {
                // Nếu đang xử lý các click trước đó, bỏ qua click hiện tại
                return;
            }

            PictureBox currentCard = (PictureBox)sender;
            string str = currentCard.Tag.ToString();
            currentCard.Image = Image.FromFile(str);

            if (firstCard == null)
            {
                firstCard = currentCard;
            }
            else
            {
                if (firstCard == currentCard)
                {
                    // Đã chọn cùng 1 thẻ, không làm gì
                    return;
                }

                if (firstCard.Tag.ToString() == currentCard.Tag.ToString())
                {
                    // Hai hình giống nhau
                    isProcessing = true; // Đánh dấu đang xử lý

                    Timer timer = new Timer();
                    timer.Interval = 10;
                    int steps = 20;
                    double opacityStep = 1.0 / steps;
                    int delay = 300 / steps;
                    int currentStep = 0;

                    timer.Tick += (s, args) =>
                    {
                        currentStep++;
                        if (currentStep <= steps)
                        {
                            double opacity = 1 - (opacityStep * currentStep);
                            currentCard.BackColor = Color.FromArgb((int)(opacity * 255), currentCard.BackColor);
                            firstCard.BackColor = Color.FromArgb((int)(opacity * 255), firstCard.BackColor);
                        }
                        else
                        {
                            currentCard.Visible = false;
                            firstCard.Visible = false;
                            timer.Stop();

                            firstCard = null;

                            matchedCardsCount += 2;
                            if (matchedCardsCount == cards.Count)
                            {
                                // Tất cả các thẻ đã khớp, chuyển sang level tiếp theo hoặc kết thúc trò chơi
                                if (level < 5) // Level tối đa
                                {
                                    level++;
                                   
                                    cols++;
                                    Controls.Clear();
                                    InitializeGame();

                                }
                                else
                                {
                                    Close();
                                }
                            }
                            // Tăng điểm và cập nhật label điểm
                                    UpdateLevelScore();
                            
                           
                            isProcessing = false; // Đánh dấu kết thúc xử lý
                        }
                    };

                    timer.Start();
                }
                else
                {
                    // Hai hình không khớp
                    isProcessing = true; // Đánh dấu đang xử lý

                    currentCard.Enabled = false;
                    Timer timer = new Timer();
                    timer.Interval = 500;
                    timer.Tick += (s, args) =>
                    {
                        timer.Stop();
                        currentCard.Image = Image.FromFile("D:\\c#\\WindowsFormsApp1\\WindowsFormsApp1\\Recources\\imagecard2.jpg");
                        firstCard.Image = Image.FromFile("D:\\c#\\WindowsFormsApp1\\WindowsFormsApp1\\Recources\\imagecard2.jpg");
                        firstCard = null;
                        currentCard.Enabled = true;
                       
                        isProcessing = false; // Đánh dấu kết thúc xử lý
                    };
                    timer.Start();
                }
            }
        }
    }
}
