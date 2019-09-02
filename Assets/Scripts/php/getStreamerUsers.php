<?php
$con = mysqli_connect('localhost','id10714824_chatfightervarcyon','Dragon20Sariou','id10714824_chatfighter');
$channelName = $_POST["channel"];


    $usersQuery = "SELECT channel FROM Streamers WHERE channel='lana_lux' AND platform='Twitch';";
    $users = mysqli_query($con,$usersQuery) or die("nothing selected");   
    echo $users;
?>