N = [57,58]; %���������� ����� (������ ��� �������������� � ������ �����), ������� �� ����� ����������� � ����.
rays = 7; %���������� �����
K = 10000;  %����� ����� k �������� ������� 
mu = zeros(rays, 2, K); %������� �������������

%������� ���������
power = [0 -1 -2 -3 -8 -17.2 -20.8];
sigma = sqrt(10.^(power./10)); % �������� ���������

%����� ������������, �������� MEDS

%������� ������������ ��������
c = createGainsByMEDS(sigma./sqrt(2), N(1));

%������� ������� ������������� ��������

%fray=800*10^6; %������� ����. (��)
%fmax = fray/(3*10^8); %������������ ������������ ��������
fmax = 300;
f = createFrequenciesByMEDS(fmax, N(1));

% ������� ����
teta = createPhasesByMEDS(N(1));

%������� ������������� 
%Ts = 5000*10^(-6); 
Ts=1/5000;



for z=1:rays
    mu(z,:,:) = createRayCoeff(c(:,:,z),f,teta,Ts,K,N);
end


srednee = zeros (7,1);
for s=1:7
    for k=1:K
        srednee(s)=srednee(s)+abs(mu(s,1,k)+1j*mu(s,2,k)).^2;
        
    end
    srednee(s)=srednee(s)/(K);
end
  
disp(srednee);
disp(sigma.^2);



 x=squeeze(mu(1,1,:)+1j*mu(1,2,:));

 M=30;
 R=correlation(x,M);

 x1=squeeze(mu(1,1,:));
 x2=squeeze(mu(1,2,:));


 hold off;
 plot(correlation(x1,M));
 hold on;
 plot(correlation(x2,M));
 plot(sigma(1)^2/2*besselj(0,2*pi*fmax*Ts*(0:M)),'r-.');