function R = correlation(x,M)
    K = length(x); % найдём длину вектора
    R=zeros(M,1);
    for m=0:M
        R(m+1)=sum(x(1:K-M).*conj(x(1+m:K-M+m))/(K-M));
    end
end

