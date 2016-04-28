data = csvread('.\..\Assets\output.csv', 1, 0);

colours = zeros(length(data(:,1)), 3);

colours(:,1) = data(:,1);

%create a second dataset, normalised by depth
newData = data;

newData(:,2)  = newData(:,2) ./ newData(:,1);
newData(:,3)  = newData(:,3) ./ newData(:,1);
newData(:,4)  = newData(:,4) ./ newData(:,1);
newData(:,5)  = newData(:,5) ./ newData(:,1);

newColours = ones(length(newData(:,1)), 3);

newColours = newData(:,1) / max(newData(:,1));

figure
scatter(data(:,2), data(:,3), 25, data(:,1));
hold on
scatter(data(:,2) + data(:,4), data(:,3) + data(:,5), 25, data(:,1)/2);
hold on
quiver(data(:,2), data(:,3), data(:,4), data(:,5));

xlabel('X Offset');
ylabel('Y Offset');
c = colorbar;
c.Label.String = 'Depth';


%figure

%scatter(newData(:,2), newData(:,3), 25, 'green');
%hold on
%scatter(newData(:,2) + newData(:,4), newData(:,3) + newData(:,5), 25, 'red');

%figure
%quiver(newData(:,2), newData(:,3), newData(:,4), newData(:,5));

figure
scatter3(data(:,2), data(:,3), data(:,1), 25, data(:,1));
hold on
scatter3(data(:,2) + data(:,4), data(:,3) + data(:,5), data(:,1), 25, data(:,1)/2);
hold on
quiver3(data(:,2), data(:,3), data(:,1), data(:,4), data(:,5), zeros(size(data(:,1))));

xlabel('X Offset');
ylabel('Y Offset');
zlabel('Depth');

%Offset over depth
figure
scatter3(data(:,4), data(:,5), data(:,1), 25, data(:,1)/2);
xlabel('X Offset');
ylabel('Y Offset');
zlabel('Depth');

%figure
%scatter3(newData(:,2), newData(:,3), newData(:,1), 25, newColours);
%hold on
%quiver3(newData(:,2), newData(:,3), newData(:,1), newData(:,4), newData(:,5), zeros(size(data(:,1))));
%view(45, 45);
%xlabel('X Offset');
%ylabel('Y Offset');
%zlabel('Depth');