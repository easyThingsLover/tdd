using System.Drawing;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;


namespace TagsCloudVisualization;

[TestFixture]
public class CircularCloudLayouterTest
{
    private CircularCloudLayouter layouter;
    private Point center;
    const int imageWidth = 1100;
    const int imageHeight = 900;

    [SetUp]
    public void SetUp()
    {
        center = new Point(100, 100);
        layouter = new CircularCloudLayouter(center);
    }
    [TearDown]
    public void TearDown()
    {
        var result = TestContext.CurrentContext.Result;
        if (result.Outcome.Status != TestStatus.Failed) return;
        var testName = TestContext.CurrentContext.Test.Name;
        var directory = TestContext.CurrentContext.TestDirectory;
        var fileName = $"{testName}_tagCloud.png";
        var path = Path.Combine(directory, fileName);
        
        var visualizer = new Visualizer(
            new CircularCloudLayouter(new Point(imageWidth / 2, imageHeight / 2)));
            
        visualizer.DrawCloud(
            new[]         {
                "Football", "Soccer", "Basketball", "Baseball", "Hockey",
                "Tennis", "Volleyball", "Rugby", "Cricket", "Golf",
                "Boxing", "MMA", "Wrestling", "Cycling", "Running",
                "Marathon", "Sprint", "Swimming", "Diving", "Rowing",
                "Skating", "Skiing", "Snowboarding", "Surfing", "Skateboard",
                "Climbing", "Bouldering", "Gymnastics", "Karate", "Judo",
                "Taekwondo", "Badminton", "TableTennis", "Handball", "WaterPolo",
                "Lacrosse", "Softball", "AmericanFootball", "Fencing", "Archery",
                "Triathlon", "Biathlon", "Decathlon", "Heptathlon", "Crossfit",
                "Powerlifting", "Weightlifting", "Bodybuilding", "Yoga", "Pilates",
                "Aerobics", "Zumba", "Parkour", "Freerun", "Motorsport",
                "FormulaOne", "Rally", "Karting", "Esports", "Chess",
                "Darts", "Bowling", "Snooker", "Billiards", "Polo",
                "Kayaking", "Canoeing", "Windsurfing", "Kitesurfing", "Paragliding",
                "Mountaineering", "TrailRunning", "Ultramarathon", "NordicWalking", "Orienteering",
                "Stadium", "Arena", "Coach", "Referee", "Captain",
                "Team", "League", "Tournament", "Championship", "Playoffs",
                "Fitness", "Training", "Warmup", "Cooldown", "Stretching",
                "Goal", "Assist", "Penalty", "Offense", "Defense",
                "SprintFinish", "Record", "Medal", "Victory", "FairPlay"
            },
            path,
            imageWidth,
            imageHeight,
            Color.FromArgb(255, 102, 0),
            Color.FromArgb(0, 28, 39),
            new("Times New Roman", 16));

        TestContext.Out.WriteLine($"Tag cloud visualization saved to file {path}");
    }
    
    [Test]
    public void FailedTest()
    {
        var size = new Size(20, 20);

        var rect = layouter.PutNextRectangle(size);

        var rectCenter = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
        rectCenter.Should().NotBe(center);
    }

    [Test]
    public void PutNextRectangle_PutsRectangleAroundCenter()
    {
        var size = new Size(20, 20);

        var rect = layouter.PutNextRectangle(size);

        var rectCenter = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
        rectCenter.Should().Be(center);
    }

    [Test]
    public void PutNextRectangle_ReturnsDifferentRectangles()
    {
        var size = new Size(20, 20);

        var r1 = layouter.PutNextRectangle(size);
        var r2 = layouter.PutNextRectangle(size);

        r1.Should().NotBe(r2);
    }

    [Test]
    public void PutNextRectangle_ReturnsNotIntersectRectangles()
    {
        var size = new Size(20, 20);
        var placed = new List<Rectangle>();

        for (var i = 0; i < 100; i++)
        {
            var next = layouter.PutNextRectangle(size);
            foreach (var prev in placed)
                next.IntersectsWith(prev).Should().BeFalse();
            placed.Add(next);
        }
    }

    [Test]
    public void PutNextRectangle_CreateDenseRectangles()
    {
        var size = new Size(20, 20);
        var rects = Enumerable.Range(0, 100)
            .Select(_ => layouter.PutNextRectangle(size))
            .ToArray();

        var centers = rects.Select(GetCenter).ToArray();
        var avgX = centers.Average(p => p.X);
        var avgY = centers.Average(p => p.Y);

        Math.Abs(avgX - center.X).Should().BeLessThan(size.Width);
        Math.Abs(avgY - center.Y).Should().BeLessThan(size.Height);
    }

    [Test]
    public void PutNextRectangle_CreateSameRectangles_WhenSameSizes()
    {
        var layouter1 = new CircularCloudLayouter(center);
        var layouter2 = new CircularCloudLayouter(center);
        var sizes = new[]
        {
            new Size(10, 10),
            new Size(20, 10),
            new Size(15, 15),
            new Size(5, 30)
        };

        var rects1 = sizes.Select(s => layouter1.PutNextRectangle(s)).ToArray();
        var rects2 = sizes.Select(s => layouter2.PutNextRectangle(s)).ToArray();

        rects1.Should().BeEquivalentTo(rects2);
    }
        
    [Test]
    public void GenerateWordCloudImage()
    {
        var words = new[]
        {
            "Forest", "River", "Mountain", "Ocean", "Sea",
            "Lake", "Valley", "Desert", "Island", "Cliff",
            "Meadow", "Field", "Prairie", "Steppe", "Canyon",
            "Waterfall", "Glacier", "Volcano", "Geyser", "Lagoon",
            "Tree", "Bush", "Grass", "Flower", "Moss",
            "Leaf", "Roots", "Branch", "Seed", "Bloom",
            "Sunrise", "Sunset", "Rain", "Snow", "Storm",
            "Thunder", "Lightning", "Clouds", "Wind", "Fog",
            "Wildlife", "Birds", "Animals", "Insects", "Fish",
            "Ecosystem", "Nature", "Landscape", "Horizon", "Sky"
        };

        var visualizer = new Visualizer(new CircularCloudLayouter(new Point(imageWidth / 2, imageHeight / 2)));

        visualizer.DrawCloud(
            words, 
            "wordCloud.png",
            imageWidth,
            imageHeight,
            Color.FromArgb(255, 102, 0),
            Color.FromArgb(0, 28, 39),
            new("Times New Roman", 16));
    }
        
    [Test]
    public void GenerateSportsWordCloudImage()
    {
        var words = new[]
        {
            "Football", "Soccer", "Basketball", "Baseball", "Hockey",
            "Tennis", "Volleyball", "Rugby", "Cricket", "Golf",
            "Boxing", "MMA", "Wrestling", "Cycling", "Running",
            "Marathon", "Sprint", "Swimming", "Diving", "Rowing",
            "Skating", "Skiing", "Snowboarding", "Surfing", "Skateboard",
            "Climbing", "Bouldering", "Gymnastics", "Karate", "Judo",
            "Taekwondo", "Badminton", "TableTennis", "Handball", "WaterPolo",
            "Lacrosse", "Softball", "AmericanFootball", "Fencing", "Archery",
            "Triathlon", "Biathlon", "Decathlon", "Heptathlon", "Crossfit",
            "Powerlifting", "Weightlifting", "Bodybuilding", "Yoga", "Pilates",
            "Aerobics", "Zumba", "Parkour", "Freerun", "Motorsport",
            "FormulaOne", "Rally", "Karting", "Esports", "Chess",
            "Darts", "Bowling", "Snooker", "Billiards", "Polo",
            "Kayaking", "Canoeing", "Windsurfing", "Kitesurfing", "Paragliding",
            "Mountaineering", "TrailRunning", "Ultramarathon", "NordicWalking", "Orienteering",
            "Stadium", "Arena", "Coach", "Referee", "Captain",
            "Team", "League", "Tournament", "Championship", "Playoffs",
            "Fitness", "Training", "Warmup", "Cooldown", "Stretching",
            "Goal", "Assist", "Penalty", "Offense", "Defense",
            "SprintFinish", "Record", "Medal", "Victory", "FairPlay"
        };

        var visualizer = new Visualizer(new CircularCloudLayouter(new Point(imageWidth / 2, imageHeight / 2)));

        visualizer.DrawCloud(
            words, 
            "wordCloudSports.png",
            imageWidth,
            imageHeight,
            Color.FromArgb(255, 102, 0),
            Color.FromArgb(0, 28, 39),
            new("Times New Roman", 16));
    }

    [Test]
    public void GenerateMedicalWordCloudImage()
    {
        var words = new[]
        { 
            "Medicine", "Health", "Doctor", "Nurse", "Surgeon", 
            "Therapist", "Cardiologist", "Neurologist", "Pediatrician", "Dentist", 
            "Oncologist", "Dermatologist", "Psychiatrist", "Radiologist", "Anesthesiologist",
            "Pharmacist", "Paramedic", "Clinician", "Internist", "Endocrinologist",
            "Hospital", "Clinic", "Ward", "ICU", "Emergency",
            "Ambulance", "OperatingRoom", "Laboratory", "Pharmacy", "Reception",
            "WaitingRoom", "RecoveryRoom", "TraumaCenter", "Outpatient", "Inpatient",
            "Diagnosis", "Treatment", "Therapy", "Surgery", "Operation",
            "Rehabilitation", "Consultation", "Checkup", "Vaccination", "Screening",
            "Prevention", "Monitoring", "FollowUp", "Referral", "Telemedicine",
            "Heart", "Lungs", "Brain", "Liver", "Kidneys",
            "Stomach", "Intestines", "Pancreas", "Skin", "Bones",
            "Muscles", "Joints", "Spine", "Blood", "ImmuneSystem",
            "Infection", "Virus", "Bacteria", "Inflammation", "Tumor",
            "Cancer", "Diabetes", "Hypertension", "Stroke", "Allergy",
            "Asthma", "Depression", "Anxiety", "Insomnia", "Obesity",
            "Symptom", "Fever", "Pain", "Cough", "Fatigue",
            "Nausea", "Headache", "Dizziness", "Bleeding", "Swelling",
            "Rash", "ShortnessOfBreath", "Palpitations", "Vomiting", "Diarrhea",
            "Vaccine", "Antibiotic", "Analgesic", "Antiseptic", "Anesthetic",
            "Hormone", "Insulin", "Sedative", "Antiviral", "Steroid",
            "Vitamin", "Supplement", "Infusion", "Injection", "Tablet",
            "Stethoscope", "Syringe", "Scalpel", "Thermometer", "Glucometer",
            "ECG", "XRay", "MRI", "CTScan", "Ultrasound",
            "Defibrillator", "Ventilator", "Wheelchair", "Bandage", "Mask",
            "Gloves", "Disinfectant", "Microscope", "TestTube", "Monitor",
            "Wellness", "Nutrition", "Hydration", "Exercise", "Hygiene",
            "Immunity", "Recovery", "FirstAid", "EmergencyCare", "PalliativeCare",
            "PublicHealth", "Epidemiology", "ClinicalTrial", "Research", "EvidenceBased"
                
        };

        var visualizer = new Visualizer(new CircularCloudLayouter(new Point(imageWidth / 2, imageHeight / 2)));

        visualizer.DrawCloud(
            words, 
            "wordCloudMedicine.png",
            imageWidth,
            imageHeight,
            Color.FromArgb(255, 102, 0),
            Color.FromArgb(0, 28, 39),
            new("Times New Roman", 16));
    }
    
    private static Point GetCenter(Rectangle rectangle)
    {
        return new Point(rectangle.Left + rectangle.Width / 2, rectangle.Top + rectangle.Height / 2);
    }
}