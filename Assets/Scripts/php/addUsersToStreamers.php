<?php
$con = mysqli_connect('localhost','id10714824_chatfightervarcyon','Dragon20Sariou','id10714824_chatfighter');
$channelName = $_POST["channel"];
$users = $_POST['users'];

$insertUsers = "INSERT INTO Streamers (channel,platform,users) VALUES('".$channelName."','Twitch','".$users."');";
mysqli_query($con,$insertUsers) or die("we're doooomed");
echo "Insertion complete";
?>