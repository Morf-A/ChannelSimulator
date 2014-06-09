N = [57,58]; %количество лучей (разное для действительной и мнимой части), которые мы будем суммировать в один.
rays = 7; %количество лучей
K = 10000;  %всего имеем k моментов времени 
mu = zeros(rays, 2, K); %таблица коэффициентов

%таблица мощностей

power = [0 -1 -2 -3 -8 -17.2 -20.8]; %EPA
%power = [-1,-1,-1,0,0,0,-3,-5,-7]; %ETU
%power = [0, -1.5, -1.4, -3.6, -0.6, -9.1, -7.0, -12.0, -16.9]; %EVA

sigma = sqrt(10.^(power./10)); % вычислим дисперсию
sigma = sigma/sqrt(2);
%Задаём коэффициенты, согласно MEDS

%зададим коэффициенты усиления
c = createGainsByMEDS(sigma./sqrt(2), N(1));

%зададим частоту доплеровского смещения

%fray=800*10^6; %частота луча. (Гц)
%fmax = fray/(3*10^8); %Максимальное доплеровское смещение
fmax = 5;
f = createFrequenciesByMEDS(fmax, N(1));

% зададим фазы
teta = createPhasesByMEDS(N(1));



Td = 1/30.72*10^-6; % Частота дискретизации сигнала
%Частота дискретизации 
 
%Ts=Td*100;
Ts = 1/5000;
%Ts = 0.000003225;


for z=1:rays
    mu(z,:,:) = createRayCoeff(c(:,:,z),f,teta,Ts,K,N);
end


signal=csvread('C:\Users\Morf\Desktop\signal.csv');

%signal = randn(1,2000)+1j.*randn(1,2000);
%time = [0, 50, 120, 200, 230, 500, 1600, 2300, 5000]; % ETU
time = [0, 30, 70, 90, 110, 190, 410]; %EPA
output = processSignal(signal,mu,time,Td);

% plot(real(signal));
% hold on;
% plot(real(output),'r-.');

pwelch(output);


%csvwrite(output,'D:\ouput.csv');

%ach = getFrequencyResponse(mu,999,time,Ts);

%plot(ach(:,1).*conj(ach(:,1)));

% srednee = zeros (7,1);
% for s=1:7
%     for k=1:K
%         srednee(s)=srednee(s)+abs(mu(s,1,k)+1j*mu(s,2,k)).^2;
%         
%     end
%     srednee(s)=srednee(s)/(K);
% end
%   
% disp(srednee);
% disp(sigma.^2);
% 
% 
% 
%  x=squeeze(mu(1,1,:)+1j*mu(1,2,:));
% 
%  M=30;
%  R=correlation(x,M);
% 
%  x1=squeeze(mu(1,1,:));
%  x2=squeeze(mu(1,2,:));
% 
% 
%  hold off;
%  plot(correlation(x1,M));
%  hold on;
%  plot(correlation(x2,M));
%  plot(sigma(1)^2/2*besselj(0,2*pi*fmax*Ts*(0:M)),'r-.');