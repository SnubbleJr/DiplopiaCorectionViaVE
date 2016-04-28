function [ output_args ] = rotationalHist(figNo, fileName, dip, rightEyeCor)
%displays a histogram of the rotational data

FID = fopen(strcat('.\..\..\CAVE Logs\',fileName));

rawData = textscan(FID, '%f (%f %f %f) (%f %f %f) (%f %f %f) (%f %f %f)', 'HeaderLines', 1, 'Delimiter',',');

data = cell2mat(rawData);

z = zeros(length(data(:,13)),1);

x = 150;
y = 150;
width = 1200;
height = 800;

Xlim = [-90 90];
bins = 80;

participant = 'Participant is normal sighted';

corEye = 'left';

if(dip == true)
    participant = 'Participant has diplopia';
end

if(rightEyeCor == true)
    corEye = 'right';
end

% 
% hFig = figure(1);
% set(hFig, 'Position', [x, y, width, height]);
% 
% hist([wrapTo180(data(:,11)),wrapTo180(data(:,12)),wrapTo180(data(:,13))])
% legend('x','y','z')
% title(sprintf('Correctional rotation around axis\nDiplopia\nCount: ~%d',length(data(:,1))))
% xlim(Xlim)
% xlabel('Angle')
% ylabel('Count')
% set(gca,'XTick',[-9:9]*10)


hFig = figure(figNo);
set(hFig, 'Position', [x, y, width, height]);

set1 = [];
set2 = [];
set3 = [];

count = 0;

for i = 1:length(data(:,1))
    %only plot if looking at something
    if(data(i,1) ~= 0)
        if(rightEyeCor == true)
            set1 = [set1;wrapTo180(data(i,11))];
            set2 = [set2;wrapTo180(data(i,12))];
            set3 = [set3;wrapTo180(data(i,13))];
        else
            set1 = [set1;wrapTo180(data(i,8))];
            set2 = [set2;wrapTo180(data(i,9))];
            set3 = [set3;wrapTo180(data(i,10))];
        end
        count = count +1;
    end
end

h1 = histogram(set1);
hold on;
h2 = histogram(set2);
hold on;
h3 = histogram(set3);
hold on;

h1.NumBins = bins;
h2.NumBins = bins;
h3.NumBins = bins;

h1.BinWidth = 0.25;
h2.BinWidth = 0.25;
h3.BinWidth = 0.25;

meanX = mean(h1.Data)
meanY = mean(h2.Data)
meanZ = mean(h3.Data)

legend('x','y','z')
title(sprintf('Correctional rotation for %s eye around axis\n%s\nCount: ~%d', corEye, participant, count))
xlim(Xlim)
xlabel('Angle')
ylabel('Count')
set(gca,'XTick',[-9:9]*5)

end

