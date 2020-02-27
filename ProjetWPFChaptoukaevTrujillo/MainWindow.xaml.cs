using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Microsoft.VisualBasic;

namespace ProjetWPFChaptoukaevTrujillo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Ville> villes = new ObservableCollection<Ville>();
        public ObservableCollection<Ellipse> markers = new ObservableCollection<Ellipse>();
        public int nbPop = 10;
        public int coefXover = 3;
        public int coefMuta = 3;
        public int nbElite = 2;
        public int nbGen = 5;
        public DBinit db = DBinit.GetInstance();
        public ParamSet p1;
        public AlgoGen pgrm = new AlgoGen();
        public List<Metrics> metrics;

        public MainWindow()
        {
            InitializeComponent();

            // On récupère en base de données la liste des villes si elle existe
            if (db.getVilles().Count != 0)
            {
                foreach(Ville ville in db.getVilles())
                {
                    // On recréé un marker pour chacune de nos villes enregistrées et on met à jour les listes
                    int radius = 5;
                    var ellipse = new Ellipse()
                    {
                        Stroke = System.Windows.Media.Brushes.Red,
                        StrokeThickness = radius * 2
                    };
                    Canvas.SetTop(ellipse, ville.posY - radius);
                    Canvas.SetLeft(ellipse, ville.posX - radius);
                    
                    canvasCarteFrance.Children.Add(ellipse);
                    markers.Add(ellipse);
                    villes.Add(ville);
                }
            }

            // On lie la liste des villes avec l'interface graphique
            listeCarte.ItemsSource = villes;

            // On récupère en base de données les paramètres s'ils existent ou bien on les créé avec des valeurs par défaut
            if(db.getParamSets().Count == 0)
            {
                p1 = new ParamSet(nbPop, coefXover, coefMuta, nbElite, nbGen);
                db.SaveParamSet(p1);
            }
            else
            {
                p1 = db.getParamSets()[0];
                nbPop = p1.Npop;
                coefXover = p1.nXover;
                coefMuta = p1.nMutant;
                nbElite = p1.nElite;
                nbGen = p1.stop;
            }

            // On lie nos paramètres avec les éléments graphiques
            taillePop.Text = nbPop.ToString();
            coefCrossover.Text = coefXover.ToString();
            coefMutation.Text = coefMuta.ToString();
            nombreElite.Text = nbElite.ToString();
            nombreGen.Text = nbGen.ToString();
        }

        private void ajouterVille(object sender, MouseButtonEventArgs e)
        {
            // On récupère la position du clic souris
            Point pos = Mouse.GetPosition((IInputElement)sender);

            // On créé un marker, on le positionne là où on a cliqué
            int radius = 5;
            var ellipse = new Ellipse()
            {
                Stroke = System.Windows.Media.Brushes.Red,
                StrokeThickness = radius*2
            };
            Canvas.SetTop(ellipse, pos.Y - radius);
            Canvas.SetLeft(ellipse, pos.X - radius);

            // On affiche une pop-up pour demander le nom de la ville
            String saisieNom = Interaction.InputBox("Veuillez saisir un nom de ville : (obligatoire)");

            // Si un nom est saisi, on affiche le marker et ajoute la ville et le marker à nos listes
            if (saisieNom.Length > 0)
            {
                canvasCarteFrance.Children.Add(ellipse);
                markers.Add(ellipse);
                villes.Add(new Ville(saisieNom, pos.X, pos.Y));
            }

            // On enregistre en base de données la liste de nos villes
            List<Ville> listeVilles = new List<Ville>();
            foreach (Ville ville in villes)
            {
                listeVilles.Add(ville);
            }
            db.SaveVilles(listeVilles);
        }

        private void supprimerVille(object sender, MouseButtonEventArgs e)
        {
            if (listeCarte.SelectedIndex != -1)
            {
                // On supprime le marker de la carte et de la liste
                canvasCarteFrance.Children.RemoveAt(listeCarte.SelectedIndex + 1);
                markers.RemoveAt(listeCarte.SelectedIndex);

                // On supprime la ville de la liste
                villes.RemoveAt(listeCarte.SelectedIndex);

                // On enregistre en base de données la liste de nos villes
                List<Ville> listeVilles = new List<Ville>();
                foreach (Ville ville in villes)
                {
                    listeVilles.Add(ville);
                }
                db.SaveVilles(listeVilles);
            }
        }

        private void lancerAlgorithme(object sender, RoutedEventArgs e)
        {
            // On convertit notre ObservableCollection en List
            List<Ville> listeVillesInitiales = new List<Ville>();
            foreach (Ville ville in villes)
            {
                listeVillesInitiales.Add(ville);
            }

            // On initialise notre liste de métriques
            metrics = new List<Metrics>();

            // On récupère les paramètres
            p1 = db.getParamSets()[0];

            // On crée la population initiale
            List<Chemin> generationInitiale = pgrm.CheminsAleatoires(p1.Npop, listeVillesInitiales);

            // On fait appel a l'algorithme qui va calculer les scores de plein de chemins.
            // On lui passe la liste "metrics" en parametre pour qu'il la remplisse.
            Chemin winner = pgrm.Evolution(p1, listeVillesInitiales, generationInitiale, metrics);

            // On lie nos métriques avec l'interface graphique
            listeGen.ItemsSource = metrics;

            // On affiche le chemin optimal
            cheminOptimal.Text = "Chemin optimal : " + winner.ToString();
        }

        // On enregistre en base de données la liste de nos paramètres
        private void majParams(object sender, RoutedEventArgs e)
        {
            nbPop = int.Parse(taillePop.Text);
            coefXover = int.Parse(coefCrossover.Text);
            coefMuta = int.Parse(coefMutation.Text);
            nbElite = int.Parse(nombreElite.Text);
            nbGen = int.Parse(nombreGen.Text);
            db.SaveParamSet(new ParamSet(nbPop, coefXover, coefMuta, nbElite, nbGen));
            MessageBox.Show("Paramètres sauvegardés !");
        }
    }
}
